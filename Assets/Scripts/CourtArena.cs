using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace JustSmash
{
    public sealed partial class CourtPrototype
    {
        readonly List<Material> arenaMaterials = new List<Material>();
        readonly List<Mesh> arenaMeshes = new List<Mesh>();
        Mesh[] seatedMeshes;
        Material[][] peopleMaterials;
        TextMesh arenaScore;
        Transform umpireArm, umpireElbow;
        Quaternion umpireArmRest, umpireElbowRest;
        float officialSignalUntil;
        string arenaScoreKey;
        SinglesMatch arenaMatch;

        Texture2D courtTexture, contactShadowTexture;
        Material contactShadowMaterial;
        void AddContactShadow(Transform parent)
        {
            if (contactShadowMaterial == null)
            {
                contactShadowTexture = new Texture2D(32, 32, TextureFormat.RGBA32, true) { name = "Soft contact shadow", wrapMode = TextureWrapMode.Clamp };
                var pixels = new Color[32 * 32];
                for (int y = 0; y < 32; y++) for (int x = 0; x < 32; x++)
                {
                    float radius = new Vector2(x / 31f * 2 - 1, y / 31f * 2 - 1).sqrMagnitude;
                    float fade = Mathf.Clamp01(1 - radius);
                    pixels[y * 32 + x] = new Color(0, 0, 0, .45f * fade * fade);
                }
                contactShadowTexture.SetPixels(pixels); contactShadowTexture.Apply();
                contactShadowMaterial = new Material(SpriteShader) { mainTexture = contactShadowTexture }; arenaMaterials.Add(contactShadowMaterial);
            }
            var shadow = GameObject.CreatePrimitive(PrimitiveType.Quad); shadow.name = "ContactShadow";
            shadow.transform.SetParent(parent, false); shadow.transform.localPosition = new Vector3(0, .004f, 0);
            shadow.transform.localRotation = Quaternion.Euler(90, 0, 0); shadow.transform.localScale = new Vector3(.85f, .65f, 1);
            var renderer = shadow.GetComponent<Renderer>(); renderer.sharedMaterial = contactShadowMaterial; renderer.shadowCastingMode = ShadowCastingMode.Off;
            Destroy(shadow.GetComponent<Collider>());
        }
        Material CourtSurface()
        {
            courtTexture = new Texture2D(64, 64, TextureFormat.RGB24, true) { name = "Court matte grain", wrapMode = TextureWrapMode.Repeat, filterMode = FilterMode.Trilinear, anisoLevel = 2 };
            var pixels = new Color[64 * 64];
            for (int y = 0; y < 64; y++) for (int x = 0; x < 64; x++)
            {
                float value = .95f + .04f * Mathf.PerlinNoise(x * .5f, y * .5f);
                pixels[y * 64 + x] = new Color(value, value, value);
            }
            courtTexture.SetPixels(pixels); courtTexture.Apply();
            var material = ArenaMaterial(new Color(.12f, .43f, .27f));
            material.mainTexture = courtTexture; material.mainTextureScale = new Vector2(80, 160); return material;
        }
        Material ArenaMaterial(Color color)
        {
            var material = Mat(color); material.enableInstancing = true;
            arenaMaterials.Add(material); return material;
        }
        Transform ArenaPart(string name, Vector3 position, Vector3 size, Material material, Transform parent = null)
        {
            var item = GameObject.CreatePrimitive(PrimitiveType.Cube); item.name = name;
            item.transform.SetParent(parent, false); item.transform.localPosition = position; item.transform.localScale = size;
            var renderer = item.GetComponent<MeshRenderer>(); renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            Destroy(item.GetComponent<Collider>()); return item.transform;
        }
        TextMesh ArenaText(string name, string text, Vector3 position, float size, Color color, Transform parent = null)
        {
            var item = new GameObject(name); item.transform.SetParent(parent, false); item.transform.localPosition = position;
            var label = item.AddComponent<TextMesh>(); label.text = text; label.fontSize = 64; label.characterSize = size;
            label.anchor = TextAnchor.MiddleCenter; label.alignment = TextAlignment.Center; label.color = color;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var material = new Material(Resources.Load<Shader>("ArenaText")) { mainTexture = label.font.material.mainTexture };
            arenaMaterials.Add(material); label.GetComponent<MeshRenderer>().sharedMaterial = material; return label;
        }
        static void PointBone(Transform bone, Transform child, Vector3 direction)
        {
            bone.rotation = Quaternion.FromToRotation(child.position - bone.position, direction) * bone.rotation;
        }
        static void PoseSeatedOfficial(Transform model)
        {
            var hips = model.Find("Root/Hips"); hips.position = model.TransformPoint(new Vector3(0, .53f, 0));
            foreach (string side in new[] { "Left", "Right" })
            {
                float sign = side == "Left" ? -1 : 1;
                var thigh = hips.Find(side + "UpLeg"); var knee = thigh.Find(side + "Leg"); var foot = knee.Find(side + "Foot");
                PointBone(thigh, knee, model.forward + model.right * sign * .15f);
                PointBone(knee, foot, -model.up + model.forward * .10f);
                var arm = hips.Find("Spine/Spine1/" + side + "Shoulder/" + side + "Arm");
                var elbow = arm.Find(side + "ForeArm"); var hand = elbow.Find(side + "Hand");
                PointBone(arm, elbow, -model.up + model.forward * .28f);
                PointBone(elbow, hand, (hips.position + model.forward * .23f + model.right * sign * .13f) - elbow.position);
            }
        }
        void BakeSeatedOfficial()
        {
            var model = Instantiate(Resources.Load<GameObject>("AthleteNPC")); model.name = "OfficialBakeSource";
            var animator = model.GetComponent<Animator>(); if (animator != null) animator.enabled = false;
            PoseSeatedOfficial(model.transform);
            var skins = model.GetComponentsInChildren<SkinnedMeshRenderer>();
            seatedMeshes = new Mesh[skins.Length]; peopleMaterials = new Material[skins.Length][];
            for (int i = 0; i < skins.Length; i++)
            {
                var mesh = new Mesh { name = "Arena seated " + skins[i].name }; skins[i].BakeMesh(mesh);
                var vertices = mesh.vertices; Matrix4x4 matrix = model.transform.worldToLocalMatrix * skins[i].transform.localToWorldMatrix;
                for (int j = 0; j < vertices.Length; j++) vertices[j] = matrix.MultiplyPoint3x4(vertices[j]);
                mesh.vertices = vertices; mesh.RecalculateNormals(); mesh.RecalculateBounds();
                seatedMeshes[i] = mesh; arenaMeshes.Add(mesh); peopleMaterials[i] = skins[i].sharedMaterials;
                for (int j = 0; j < peopleMaterials[i].Length; j++)
                {
                    var material = new Material(peopleMaterials[i][j]); material.shader = LitShader;
                    material.SetFloat("_Flat", skins[i].name.Contains("tshirt") ? 1 : 0);
                    peopleMaterials[i][j] = material; arenaMaterials.Add(material);
                }
            }
            model.SetActive(false); Destroy(model);
        }
        Transform SeatedPerson(string name, Vector3 position, float yaw, Transform parent = null)
        {
            var person = new GameObject(name).transform; person.SetParent(parent, false);
            person.localPosition = position; person.localRotation = Quaternion.Euler(0, yaw, 0);
            for (int i = 0; i < seatedMeshes.Length; i++)
            {
                var part = new GameObject("Body"); part.transform.SetParent(person, false);
                part.AddComponent<MeshFilter>().sharedMesh = seatedMeshes[i];
                var renderer = part.AddComponent<MeshRenderer>(); renderer.sharedMaterials = peopleMaterials[i]; renderer.shadowCastingMode = ShadowCastingMode.Off;
                if (seatedMeshes[i].name.Contains("tshirt"))
                {
                    var block = new MaterialPropertyBlock();
                    block.SetColor("_Color", new Color(.10f, .16f, .22f));
                    renderer.SetPropertyBlock(block);
                }
            }
            return person;
        }
        Transform RiggedPerson(string name, Vector3 position, float yaw, Transform parent = null)
        {
            var person = Instantiate(Resources.Load<GameObject>("AthleteNPC"), parent).transform; person.name = name;
            person.SetPositionAndRotation(position, Quaternion.Euler(0, yaw, 0));
            var animator = person.GetComponent<Animator>(); if (animator != null) animator.enabled = false;
            PoseSeatedOfficial(person);
            var skins = person.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < skins.Length; i++)
            {
                skins[i].sharedMaterials = peopleMaterials[i]; skins[i].updateWhenOffscreen = true;
                if (!skins[i].name.Contains("tshirt")) continue;
                var block = new MaterialPropertyBlock();
                block.SetColor("_Color", new Color(.10f, .16f, .22f)); skins[i].SetPropertyBlock(block);
            }
            return person;
        }
        const float NetDepth = .76f, NetTapeHeight = .075f;
        static float NetTop(float x)
        {
            float t = Mathf.Clamp(x / BootDiagnostics.Current.doublesHalfWidth, -1, 1);
            return BootDiagnostics.Current.netHeight - .026f * (1 - t * t);
        }
        void BuildNet(Color white)
        {
            const int columns = 64;
            float width = BootDiagnostics.Current.doublesHalfWidth;
            var vertices = new List<Vector3>(); var indices = new List<int>();
            void Strip(Vector3 a, Vector3 b, Vector3 offset)
            {
                int i = vertices.Count; vertices.Add(a); vertices.Add(b); vertices.Add(b + offset); vertices.Add(a + offset);
                vertices.Add(a); vertices.Add(b); vertices.Add(b + offset); vertices.Add(a + offset);
                indices.AddRange(new[] { i, i + 1, i + 2, i, i + 2, i + 3, i + 6, i + 5, i + 4, i + 7, i + 6, i + 4 });
            }
            void Finish(string name, Color color)
            {
                var mesh = new Mesh { name = name }; mesh.SetVertices(vertices); mesh.SetTriangles(indices, 0); mesh.RecalculateNormals(); mesh.RecalculateBounds(); arenaMeshes.Add(mesh);
                var item = new GameObject(name); item.AddComponent<MeshFilter>().sharedMesh = mesh;
                var renderer = item.AddComponent<MeshRenderer>(); renderer.sharedMaterial = ArenaMaterial(color); renderer.shadowCastingMode = ShadowCastingMode.Off;
                vertices.Clear(); indices.Clear();
            }
            for (int i = 0; i < columns; i++)
            {
                float a = Mathf.Lerp(-width, width, i / (float)columns), b = Mathf.Lerp(-width, width, (i + 1f) / columns);
                Strip(new Vector3(a, NetTop(a), 0), new Vector3(b, NetTop(b), 0), Vector3.down * NetTapeHeight);
            }
            Finish("NetTape", white);
            // ponytail: coarse render lattice keeps two draw calls; use authored mesh texture if distant aliasing persists.
            const float cord = .005f;
            for (int i = 0; i <= columns; i++)
            {
                float x = Mathf.Lerp(-width, width, i / (float)columns);
                float a = Mathf.Clamp(x - cord * .5f, -width, width), b = Mathf.Clamp(x + cord * .5f, -width, width);
                Strip(new Vector3(a, NetTop(a) - NetTapeHeight, 0), new Vector3(b, NetTop(b) - NetTapeHeight, 0), Vector3.down * (NetDepth - NetTapeHeight));
            }
            for (int row = 1; row <= 8; row++) for (int i = 0; i < columns; i++)
            {
                float a = Mathf.Lerp(-width, width, i / (float)columns), b = Mathf.Lerp(-width, width, (i + 1f) / columns);
                float depth = Mathf.Lerp(NetTapeHeight, NetDepth, row / 8f);
                Strip(new Vector3(a, NetTop(a) - depth, 0), new Vector3(b, NetTop(b) - depth, 0), Vector3.up * cord);
            }
            Finish("NetMesh", new Color(.12f, .15f, .16f));
            float height = BootDiagnostics.Current.netHeight;
            foreach (float x in new[] { -width, width }) Shape("NetPost", PrimitiveType.Cylinder, new Vector3(x, height * .5f, 0), new Vector3(.05f, height * .5f, .05f), white);
        }
        void BuildAudience()
        {
            var material = new Material(SpriteShader) { name = "Tribune silhouettes" };
            arenaMaterials.Add(material);
            var audience = new GameObject("TribuneAudience").transform;
            foreach (int side in new[] { -1, 1 })
            {
                foreach (int end in new[] { -1, 1 })
                    BuildAudienceSection(audience, material, new Vector3(side * 6.45f, 0, end * 4.6f), Vector3.back * side, Vector3.right * side);
                BuildAudienceSection(audience, material, new Vector3(side * 6.15f, 0, 12.8f), Vector3.right, Vector3.forward);
            }
        }
        void BuildAudienceSection(Transform parent, Material material, Vector3 position, Vector3 across, Vector3 rear)
        {
            const int seatsPerRow = 8;
            const float spacing = .57f;
            var vertices = new List<Vector3>();
            var colors = new List<Color32>();
            var triangles = new List<int>();
            var muted = new Color32(107, 123, 147, 255);
            var cool = new Color32(83, 111, 131, 255);
            var warm = new Color32(134, 123, 128, 255);
            var fascia = new Color32(39, 56, 72, 255);
            var section = new GameObject("TribuneSection");
            section.transform.SetParent(parent, false);
            section.transform.localPosition = position;

            for (int row = 1; row >= 0; row--)
            {
                Vector3 ledge = rear * (row * .82f) + Vector3.up * (.53f + row * .44f);
                float halfWidth = seatsPerRow * spacing / 2 + .14f;
                Vector3 band = ledge - rear * .15f;
                Quad(band, halfWidth, halfWidth, -.47f, .02f, fascia);
                for (int seat = 0; seat < seatsPerRow; seat++)
                {
                    int pattern = seat + row * 3 + (position.x < 0 ? 2 : 0) + (position.z < 0 ? 1 : 0);
                    if (pattern % 7 == 3) continue;
                    float x = (seat - (seatsPerRow - 1) * .5f + row * .22f) * spacing;
                    float height = (pattern % 3 - 1) * .035f;
                    var tint = pattern % 4 == 0 ? warm : pattern % 2 == 0 ? muted : cool;
                    Vector3 person = ledge + across * x;
                    Quad(person, .12f, .17f, 0, .30f + height, tint);
                    int head = vertices.Count;
                    Vector3 center = person + Vector3.up * (.40f + height);
                    for (int corner = 0; corner < 8; corner++)
                    {
                        float angle = corner * (Mathf.PI / 4f);
                        AddVertex(center + across * (Mathf.Cos(angle) * .093f) + Vector3.up * (Mathf.Sin(angle) * .105f), tint);
                    }
                    for (int corner = 1; corner < 7; corner++) Triangle(head, head + corner, head + corner + 1);
                }
            }
            var mesh = new Mesh { name = "Tribune section silhouettes" };
            mesh.SetVertices(vertices);
            mesh.SetColors(colors);
            mesh.SetTriangles(triangles, 0);
            mesh.uv = new Vector2[vertices.Count];
            arenaMeshes.Add(mesh);
            section.AddComponent<MeshFilter>().sharedMesh = mesh;
            var renderer = section.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;

            void Quad(Vector3 origin, float baseWidth, float topWidth, float bottom, float top, Color32 tint)
            {
                int start = vertices.Count;
                AddVertex(origin - across * baseWidth + Vector3.up * bottom, tint);
                AddVertex(origin + across * baseWidth + Vector3.up * bottom, tint);
                AddVertex(origin + across * topWidth + Vector3.up * top, tint);
                AddVertex(origin - across * topWidth + Vector3.up * top, tint);
                Triangle(start, start + 1, start + 2);
                Triangle(start, start + 2, start + 3);
            }
            void AddVertex(Vector3 point, Color32 tint)
            {
                vertices.Add(point);
                colors.Add(tint);
            }
            void Triangle(int a, int b, int c)
            {
                triangles.Add(a);
                triangles.Add(b);
                triangles.Add(c);
            }
        }
        void BuildArena()
        {
            var ink = ArenaMaterial(new Color(.045f, .075f, .09f));
            var steel = ArenaMaterial(new Color(.28f, .34f, .36f));
            var seats = ArenaMaterial(new Color(.12f, .24f, .26f));
            var teal = ArenaMaterial(new Color(.094f, .345f, .925f));
            // CelCourt ignores ambient/intensity: finish the hall without flooding gameplay.
            GameObject.Find("HallFloor").GetComponent<Renderer>().sharedMaterial.color = new Color(.11f, .17f, .24f);
            GameObject.Find("HallBackWall").GetComponent<Renderer>().sharedMaterial.color = new Color(.15f, .21f, .28f);
            var lining = ArenaMaterial(new Color(.07f, .11f, .17f));
            ArenaPart("HallWallLining", new Vector3(0, .72f, 15.78f), new Vector3(19.6f, 1.44f, .12f), lining);
            ArenaPart("HallLiningCap", new Vector3(0, 1.46f, 15.74f), new Vector3(19.6f, .04f, .20f), steel);
            // ponytail: unlit diffusers imply fixtures; keep the single existing key light.
            var paper = new Material(SpriteShader) { name = "Ceiling diffuser", color = new Color(.70f, .76f, .78f) };
            arenaMaterials.Add(paper);
            BuildAudience();
            BakeSeatedOfficial();
            foreach (int side in new[] { -1, 1 })
            {
                ArenaPart("ArenaBarrier", new Vector3(side * 4.7f, .38f, .7f), new Vector3(.15f, .76f, 17), ink);
                foreach (float z in new[] { -4f, 1f, 6f })
                {
                    var sign = ArenaText("SidelineBrand", "JUST SMASH", new Vector3(side * 4.58f, .43f, z), .065f, Color.white);
                    sign.transform.rotation = Quaternion.Euler(0, side < 0 ? -90 : 90, 0);
                }
                ArenaPart("HallWallLining", new Vector3(side * 9.78f, .72f, 0), new Vector3(.12f, 1.44f, 28), lining);
                ArenaPart("HallLiningCap", new Vector3(side * 9.74f, 1.46f, 0), new Vector3(.20f, .04f, 28), steel);
                for (int i = 0; i < 5; i++)
                {
                    ArenaPart("CeilingLampHousing", new Vector3(side * 4.5f, 7.08f, -8 + i * 4), new Vector3(.46f, .12f, 1.6f), ink);
                    ArenaPart("CeilingLamp", new Vector3(side * 4.5f, 7, -8 + i * 4), new Vector3(.30f, .08f, 1.4f), paper);
                }
            }
            var chair = new GameObject("UmpireChair").transform; chair.position = new Vector3(-4.05f, 0, 0);
            ArenaPart("ChairSeat", new Vector3(0, 1.75f, 0), new Vector3(.7f, .12f, .65f), teal, chair);
            foreach (float x in new[] { -.28f, .28f }) foreach (float z in new[] { -.25f, .25f })
                ArenaPart("ChairLeg", new Vector3(x, .88f, z), new Vector3(.07f, 1.76f, .07f), steel, chair);
            for (int i = 0; i < 5; i++) ArenaPart("ChairStep", new Vector3(-.32f, .22f + i * .3f, 0), new Vector3(.08f, .05f, .6f), steel, chair);
            ArenaPart("ChairBack", new Vector3(-.32f, 2.02f, 0), new Vector3(.08f, .5f, .65f), teal, chair);
            var umpire = RiggedPerson("Umpire", new Vector3(-4.05f, 1.4f, 0), 90);
            umpireArm = umpire.Find("Root/Hips/Spine/Spine1/RightShoulder/RightArm");
            umpireElbow = umpireArm.Find("RightForeArm");
            umpireArmRest = umpireArm.localRotation; umpireElbowRest = umpireElbow.localRotation;
            ArenaPart("JudgeSeat", new Vector3(4, .37f, -.25f), new Vector3(.65f, .12f, .65f), teal);
            SeatedPerson("ServiceJudge", new Vector3(4, 0, -.25f), -90);
            ArenaPart("ArenaScoreboard", new Vector3(0, 3.15f, 15.2f), new Vector3(7.8f, 3.4f, .14f), ink);
            arenaScore = ArenaText("ArenaScoreText", "JUST SMASH\nTUNGGAL PUTRA\n0     0", new Vector3(0, 3.15f, 15.10f), .12f, Color.white, GameObject.Find("ArenaScoreboard").transform);
            arenaScore.transform.localPosition = new Vector3(0, 0, -.6f); arenaScore.transform.localScale = new Vector3(1 / 7.8f, 1 / 3.4f, 1 / .14f);
            ArenaPart("VenueStripe", new Vector3(0, 1.9f, 15.85f), new Vector3(20, .14f, .03f), teal);
            ArenaText("VenueName", "GELANGGANG JUST SMASH", new Vector3(0, 5.5f, 15.80f), .21f, new Color(.65f, .76f, .76f));
            foreach (int side in new[] { -1, 1 })
            {
                var bench = ArenaPart("PlayerBench", new Vector3(side * 4.1f, .38f, -6.7f), new Vector3(.5f, .12f, 1.6f), seats);
                ArenaPart("KitBag", bench.position + new Vector3(0, .18f, .3f), new Vector3(.36f, .28f, .55f), ink);
            }
        }
        void UpdateArena()
        {
            if (arenaScore == null) return;
            string key = Match == null ? MatchTitle : Match.GameNumber + ":" + Match.Points[0] + ":" + Match.Points[1] + ":" + Match.Games[0] + ":" + Match.Games[1];
            if (arenaMatch != Match) { arenaMatch = Match; arenaScoreKey = null; officialSignalUntil = 0; }
            if (key != arenaScoreKey)
            {
                if (arenaScoreKey != null && Match != null) officialSignalUntil = Time.time + (Match.Winner >= 0 ? 5 : 1.5f);
                arenaScoreKey = key;
                arenaScore.text = "JUST SMASH\n" + (Match == null ? MatchTitle : "GAME " + Match.GameNumber + "   " + Match.Games[0] + " : " + Match.Games[1]) + "\n" + (Match == null ? "0     0" : Match.Points[0] + "     " + Match.Points[1]);
            }
            if (Time.timeScale == 0 || replaying) return;
            float cheer = Mathf.Clamp01((officialSignalUntil - Time.time) * 2);
            umpireArm.localRotation = umpireArmRest; umpireElbow.localRotation = umpireElbowRest;
            if (cheer > 0)
            {
                var official = umpireArm.root;
                PointBone(umpireArm, umpireElbow, official.forward + official.right * .35f);
                PointBone(umpireElbow, umpireElbow.Find("RightHand"), official.forward + official.up * .15f);
                umpireArm.localRotation = Quaternion.Slerp(umpireArmRest, umpireArm.localRotation, cheer);
                umpireElbow.localRotation = Quaternion.Slerp(umpireElbowRest, umpireElbow.localRotation, cheer);
            }
        }
        void ReleaseArena()
        {
            Destroy(courtTexture); Destroy(contactShadowTexture);
            foreach (var mesh in arenaMeshes) Destroy(mesh);
            foreach (var material in arenaMaterials) Destroy(material);
        }
    }
}

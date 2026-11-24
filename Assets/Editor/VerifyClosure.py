"""Run after final build: python Assets/Editor/VerifyClosure.py"""
from pathlib import Path
import hashlib
import json
import subprocess
import zipfile
import xml.etree.ElementTree as ET

sdk = Path('C:/Program Files/Unity/Hub/Editor/6000.3.18f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK')
apk = Path('builds/JustSmash-M2.apk')
aapt = next(sdk.glob('build-tools/*/aapt.exe'))
def dump(*args):
    return subprocess.run([str(aapt), 'dump', *args], capture_output=True, text=True, check=True).stdout
badging = dump('badging', str(apk))
manifest = dump('xmltree', str(apk), 'AndroidManifest.xml')
assert "name='com.justsmash.prototype'" in badging
assert "native-code: 'arm64-v8a'" in badging
assert 'android.hardware.screen.landscape' in badging
permissions = [s for s in badging.splitlines() if 'uses-permission:' in s]
assert permissions == ["uses-permission: name='com.justsmash.prototype.DYNAMIC_RECEIVER_NOT_EXPORTED_PERMISSION'"], permissions
orientation = [s.strip() for s in manifest.splitlines() if 'screenOrientation' in s]
assert any('(type 0x10)0x0' in s for s in orientation), orientation  # Android landscape=0
assert apk.stat().st_size < 1_000_000_000
suites = {}
for mode in ['play', 'edit']:
    root = ET.parse(f'evidence/M2/closure-final-{mode}.xml').getroot()
    assert root.attrib['result'] == 'Passed' and int(root.attrib['total']) > 0
    assert root.attrib['failed'] == root.attrib['skipped'] == '0'
    suites[mode] = {k: root.attrib[k] for k in ['total', 'passed', 'failed', 'skipped']}
source = json.loads(Path('evidence/M2/closure-source-manifest.json').read_text())
# Unity Test Runner creates/deletes temporary scene; not production input.
source = {p: h for p, h in source.items() if not Path(p).name.startswith('InitTestScene')}
changed = [p for p, h in source.items() if not Path(p).is_file() or hashlib.sha256(Path(p).read_bytes()).hexdigest() != h]
assert not changed, changed
archive = zipfile.ZipFile(apk)
result = dict(status='PASS', apk=str(apk.resolve()), bytes=apk.stat().st_size,
              sha256=hashlib.sha256(apk.read_bytes()).hexdigest(), permissions=permissions,
              architecture='arm64-v8a', orientation=orientation,
              uncompressedZipBytes=sum(i.file_size for i in archive.infolist()),
              sourceFilesUnchanged=len(source), bootSceneUnchanged=True, suites=suites,
              installedFootprint='NOT_RUN', physicalDevice='NOT_RUN',
              buildReport=Path('evidence/M2/build-result.txt').read_text())
Path('evidence/M2/closure-final-apk.json').write_text(json.dumps(result, indent=2))
print(json.dumps(result, indent=2))

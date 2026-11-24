"""Run: python Assets/Editor/VerifyApk.py"""
from pathlib import Path
import subprocess
import hashlib
import json

sdk = Path('C:/Program Files/Unity/Hub/Editor/6000.3.18f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK')
apk = Path('builds/JustSmash-M0.apk')
aapt = next(sdk.glob('build-tools/*/aapt.exe'))
r = subprocess.run([str(aapt), 'dump', 'badging', str(apk)], capture_output=True, text=True, check=True)
assert "name='com.justsmash.prototype'" in r.stdout
assert "native-code: 'arm64-v8a'" in r.stdout
assert "android.hardware.screen.landscape" in r.stdout
assert "android.permission.INTERNET" not in r.stdout, 'Offline APK still requests INTERNET'
assert apk.stat().st_size < 1_000_000_000
result = dict(status='PASS', bytes=apk.stat().st_size, sha256=hashlib.sha256(apk.read_bytes()).hexdigest(), checks=['package', 'ARM64', 'landscape', 'no INTERNET', 'APK under 1 GB; installed footprint NOT_RUN'])
Path('evidence/M0/apk-verification.json').write_text(json.dumps(result, indent=2))
print(json.dumps(result, indent=2))

import subprocess
import json
import sys

package_file_path = 'Assets/MirageSDK/package.json'

commit_hash = subprocess.check_output(['git', 'rev-parse', 'HEAD']).decode().strip()

try:
    git_tag = subprocess.check_output(['git', 'describe', '--exact-match', '--tags', commit_hash]).decode().strip()
except subprocess.CalledProcessError:
    print('git tag was not found for commit ' + commit_hash)
    sys.exit(0)

if git_tag and len(git_tag) > 0:

    new_package_version = git_tag.replace("v", "")

    with open(package_file_path, 'r') as f:
        # Load the JSON object into a dictionary
        data = json.load(f)

    prev_package_version = data['version'] if 'version' in data else 'none'
    data['version'] = new_package_version

    if new_package_version != prev_package_version:
        with open(package_file_path, 'w') as f:
            # Dump the modified dictionary back to the file
            json.dump(data, f, indent=2)

        print(f'version changed from {prev_package_version} to {new_package_version}')
    else:
        print(f'version {new_package_version} did not change')

else:
    print('git tag was none or empty for commit ' + commit_hash)
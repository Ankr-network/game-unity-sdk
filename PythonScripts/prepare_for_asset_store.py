import os
import shutil
import json


def remove_dir(path):
    """
    Recursively remove directory and its contents.
    """
    if os.path.exists(path):
        shutil.rmtree(path)


def remove_file(path):
    """
    Remove a file at the given path.
    """
    if os.path.exists(path):
        os.remove(path)


def remove_assets_and_folders(project_path):
    """
    Remove all folders in the Assets folder except MirageSDK folder.
    """

    assets_dir = os.path.join(project_path, "Assets")
    for root, dirs, files in os.walk(assets_dir):
        for name in dirs:
            if root == assets_dir and name != "MirageSDK":
                print("removing folder " + name + " in the Assets folder...")
                remove_dir(os.path.join(root, name))
                remove_file(os.path.join(root, name + ".meta"))

    # Remove MirageSDK/package.json and MirageSDK/Resources/own-version-knowledge.json
    package_file = os.path.join(assets_dir, "MirageSDK", "package.json")
    own_version_file = os.path.join(assets_dir, "MirageSDK", "Resources", "own-version-knowledge.json")
    remove_file(package_file)
    remove_file(own_version_file)
    remove_file(package_file + ".meta")
    remove_file(own_version_file + ".meta")

    # Remove Resources folder if it is empty
    resources_dir = os.path.join(assets_dir, "MirageSDK", "Resources")
    if os.path.exists(resources_dir) and not os.listdir(resources_dir):
        print("removing dir " + resources_dir)
        remove_dir(resources_dir)
        remove_file(resources_dir + ".meta")
    else:
        print("failed to remove " + resources_dir)


def put_sdk_folder_to_mirage_folder(project_path):
    mirage_sdk_dir = os.path.join(project_path, "Assets", "MirageSDK")

    if os.path.exists(mirage_sdk_dir):
        mirage_dir = os.path.join(project_path, "Assets", "Mirage")
        remove_dir(mirage_dir)
        os.mkdir(mirage_dir)
        shutil.move(mirage_sdk_dir, mirage_dir)
        remove_file(mirage_sdk_dir + ".meta")


def remove_packages(project_path):
    remove_packages = ["com.unity.mobile.android-logcat", "com.unity.test-framework", "com.unity.ide.rider"]

    # Remove package references from manifest.json and packages-lock.json files
    for file_name in ["manifest.json", "packages-lock.json"]:
        file_path = os.path.join(project_path, "Packages", file_name)
        with open(file_path, "r") as file:
            data = json.load(file)

        for package in remove_packages:
            if package in data["dependencies"]:
                print(f"Removing package from {file_name}:", package)
                data[f"dependencies"].pop(package, None)

        with open(file_path, "w") as file:
            json.dump(data, file, indent=4)


if __name__ == "__main__":
    file_path = os.path.abspath(__file__)
    dir_path = os.path.dirname(file_path)
    parent_dir_path = os.path.abspath(os.path.join(dir_path, os.pardir))

    remove_assets_and_folders(parent_dir_path)
    put_sdk_folder_to_mirage_folder(parent_dir_path)
    remove_packages(parent_dir_path)

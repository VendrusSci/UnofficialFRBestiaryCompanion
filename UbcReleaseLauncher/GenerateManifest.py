import argparse
import json
import hashlib
import os


class ManifestData(object):
    def __init__(self, launcher_zip, bestiary_zip, familiar_data_zip, view_icons_zip,
                 display_icons_zip, icons_zip, images_zip, launcher_images_zip):
        self.launcher_zip = launcher_zip
        self.bestiary_zip = bestiary_zip
        self.familiar_data_zip = familiar_data_zip
        self.view_icons_zip = view_icons_zip
        self.display_icons_zip = display_icons_zip
        self.icons_zip = icons_zip
        self.images_zip = images_zip
        self.launcher_images_zip = launcher_images_zip

    def to_blob(self):
        return {
            "UBCLauncherZip": self.launcher_zip,
            "UnofficialBestiaryCompanionZip": self.bestiary_zip,
            "FamiliarDataZip": self.familiar_data_zip,
            "ViewIconsZip": self.view_icons_zip,
            "DisplayIconsZip": self.display_icons_zip,
            "IconsZip": self.icons_zip,
            "ImagesZip": self.images_zip,
            "LauncherImagesZip": self.launcher_images_zip
        }

    @staticmethod
    def from_blob(blob):
        return ManifestData(
            launcher_zip=blob["UBCLauncherZip"],
            bestiary_zip=blob["UnofficialBestiaryCompanionZip"],
            familiar_data_zip=blob["FamiliarDataZip"],
            view_icons_zip=blob["ViewIconsZip"],
            display_icons_zip=blob["DisplayIconsZip"],
            icons_zip=blob["IconsZip"],
            images_zip=blob["ImagesZip"],
            launcher_images_zip=blob["LauncherImagesZip"]
        )


def hash_file(file_name):
    hash_algorithm = hashlib.sha256()
    with open(file_name, "rb") as f:
        for chunk in iter(lambda: f.read(4096), b""):
            hash_algorithm.update(chunk)
    return hash_algorithm.hexdigest()


def main():
    parser = argparse.ArgumentParser(
        description="utility for generating manifests for UBC"
    )
    parser.add_argument(
        'release_path', help='path to release files to generate manifest for'
    )
    args = parser.parse_args()

    release_path = args.release_path
    launcher_exe_path = os.path.join(release_path, "UBCLauncher.zip")
    bestiary_exe_path = os.path.join(release_path, "UnofficialBestiaryCompanion.zip")
    familiar_data_zip_path = os.path.join(release_path, "FamiliarData.zip")
    view_icons_zip_path = os.path.join(release_path, "ViewIcons.zip")
    display_icons_zip_path = os.path.join(release_path, "DisplayIcons.zip")
    icons_zip_path = os.path.join(release_path, "Icons.zip")
    images_zip_path = os.path.join(release_path, "Images.zip")
    launcher_images_zip_path = os.path.join(release_path, "LauncherImages.zip")

    manifest_data = ManifestData(
        hash_file(launcher_exe_path),
        hash_file(bestiary_exe_path),
        hash_file(familiar_data_zip_path),
        hash_file(view_icons_zip_path),
        hash_file(display_icons_zip_path),
        hash_file(icons_zip_path),
        hash_file(images_zip_path),
        hash_file(launcher_images_zip_path)
    )
    json_string = json.dumps(manifest_data.to_blob(), indent=2, sort_keys=True)

    with open(os.path.join(release_path, "manifest.txt"), "w") as f:
        f.write(json_string)


if __name__ == "__main__":
    main()

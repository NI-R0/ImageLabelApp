# 🖼️ Image Label App

A lightweight Windows application that allows you to **label images via the right-click context menu** in File Explorer. Labels `Favourites` and `Me` can be added or removed, and the app keeps track of them in a local SQLite database. The labeled images are automatically copied into virtual folders based on their labels.

---

## 📦 Features

- Add or remove labels to images from the Windows context menu.
- Store label metadata in a local SQLite database.
- Automatically create categorized folders for each label containing copies of labeled images.
- Works even if original images are stored on external drives.
- No internet or cloud needed — fully offline and self-contained.

---

## 🚀 Quick Start

### 1. **Installation**
- Clone the repository and adjust the code so that it supports your desired labels.
- Rebuild the project.
- Place the compiled `.exe` and required `.dll`s in a folder.
- Open the folder in Powershell as Administrator and run `& ".\ImageLabelApp.exe" install`. After right-clicking any image you should now see the options `Add Label` and `Remove Label`.

### 2. **Usage**
- Right-click any image and choose `Add Label`/`Remove Label` → `YourLabelName`. <br/>
  **NOTE:** Doing this for the first time will create the database file under `%APPDATA%\Roaming\ImageLabelApp\labels.db` as well as the label folders under `%USER%\Pictures\ImageLabelApp\`.

### 3. Uninstall
- Open the installation folder in Powershell as Administrator and run `& ".\ImageLabelApp" uninstall`. This will take care of removing the label folders as well as the database folder.
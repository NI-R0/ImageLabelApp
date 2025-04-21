# 🖼️ Image Label App

A lightweight Windows application that allows you to **label images via the right-click context menu** in File Explorer. Label `Favourites` is preset but one can create as many labels as he wants; the app keeps track of them in a local SQLite database. The labeled images are automatically copied into virtual folders based on their labels.

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
- Clone the repository and build the project using Visual Studio.
- Place the compiled `.exe` and required `.dll`s in a folder.
- Run the `.exe` (you'll need administrator rights) click on `install`. After right-clicking any image you should now see the menu options `Add Label` and `Remove Label`.

### 2. **Usage**
- Right-click any image and choose `Add Label`/`Remove Label` → `YourLabelName`. <br/>
  **NOTE:** Doing this for the first time will create the database file under `%APPDATA%\Roaming\ImageLabelApp\labels.db` as well as the label folders under `%USER%\Pictures\Labels\`.

### 3. Uninstall
- Run the `.exe` and click on `uninstall`. This will take care of removing the label folders as well as the database folder.
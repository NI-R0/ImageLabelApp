# 🖼️ Image Label App

A lightweight Windows application that allows you to **label images via the right-click context menu** in File Explorer. Label `Favourites` is preset but one can create as many labels as they want; the app keeps track of them in a local SQLite database. The labeled images are automatically copied into a hidden and can be viewed via the WebUI at `localhost:50505`.

---

## 📦 Features

- Add or remove labels to images from the Windows context menu.
- Store label metadata in a local SQLite database.
- Works even if original images are stored on external drives.
- No internet or cloud needed — fully offline and self-contained.
- View and filter labeled images via a separate WebUI - with the ability to create a local copy of the filtered images.

---

## 🚀 Quick Start

### 1. **Installation**
- Download the release ZIP and unzip into an installation folder.
- Run the `.\LabelApp\LabelApp.exe` (you'll need administrator rights) and click on `install`. After right-clicking any image you should now see the menu options `Add Label` and `Remove Label`.
- Create a task in the Windows Task Scheduler that starts the program `.\WebUI\StartWebUI.vbs` on user logon (you need to adjust the path in the script to your installation path beforehand). Enable options `hidden` and `run with highest privileges`.

### 2. **Usage**
- Right-click any image and choose `Add Label`/`Remove Label` → `YourLabelName`. <br/>
  **NOTE:** Doing this for the first time will create the database file under `%APPDATA%\Roaming\ImageLabelApp\labels.db` as well as the label folder under `%APPDATA%\Roaming\ImageLabelApp\Images\`.

### 3. Uninstall
- Run the `.exe` and click on `uninstall`. This will take care of removing the label folders, the database folder, as well as the context menu entries.
- Manually delete the Task Scheduler task.
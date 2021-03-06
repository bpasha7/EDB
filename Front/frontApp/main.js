'use strict';
const { app, BrowserWindow } = require('electron')

let win;

// Enable live reload for Electron too
// require('electron-reload')(`${__dirname}/dist/fontApp`, {
//   electron: require(`${__dirname}/node_modules/electron`) 
// }) 

function createWindow () {
  // Create the browser window.
  win = new BrowserWindow({
    width: 800, 
    height: 600,
    backgroundColor: '#ffffff',
    icon: `file://${__dirname}/dist/assets/logo.png`
  })

  win.setMenu(null);
  process.env['ELECTRON_DISABLE_SECURITY_WARNINGS'] = 'true';

   const basepath = app.getAppPath();
  win.loadURL(`file://${basepath}/dist/frontApp/index.html`)

  //// uncomment below to open the DevTools.
  // win.webContents.openDevTools()

  // Event when the window is closed.
  win.on('closed', function () {
    win = null
  })
}

// Create window on electron intialization
app.on('ready', createWindow)

// Quit when all windows are closed.
app.on('window-all-closed', function () {

  // On macOS specific close process
  if (process.platform !== 'darwin') {
    app.quit()
  }
})

app.on('activate', function () {
  // macOS specific close process
  if (win === null) {
    createWindow()

  }
})
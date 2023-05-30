
 -- SidecarNativeCLI --------
| SidecarNativeComm (client) |
 ---------------------------
     |
 -- SidecarNativeApp --------
| SidecarNativeComm (server) |
| NativeMessaging            |
 ----------------------------
     |
 -- Browser process --
| browser extension   |
 ---------------------

SidecarNativeCLI - CLI tool and example project that uses SidecarNativeComm to communicate with native messaging process.
SidecarNativeComm - Library for communicating cross process with the client app (SidecarNativeCLI) and NativeMessaging.
SidecarNativeApp - Browser extension native app that is started by browser extension and communicates cross process with SidecarNativeComm

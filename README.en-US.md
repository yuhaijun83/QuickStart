# QuickStart
A quick-launch tool that gathers your frequently used programs and commands in one place.

[简体中文](README.md) · [繁體中文](README.zh-TW.md) · [日本語](README.ja-JP.md) · **English** · [한국어](README.ko-KR.md)

【Build Environment】：

       Windows 11 Enterprise 26H2 (Cn) + Visual Studio 2026 Enterprise + .Net FrameWork 4.8

【Requirements】：

   .Net FrameWork 4.7.2 or above must be installed (Windows 10 LTSC 2019/2021 ships with it; older systems need a manual install)

   Download Url: [.NET Framework 4.7.2 download page](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472)

【Usage】：
  
       1.Edit the ini configuration file at any time to change the launch information of the executable programs.
       
       2.Ini changes take effect automatically: while running, the program watches the ini file and,
         after it is edited and saved externally, automatically reloads and refreshes the buttons within
         about one second (names, visibility, enable/disable, colors, program paths, order, etc.).
         No restart is needed.
       
       3.Buttons are displayed one by one according to the number configured in the ini file, up to 8 per row, with overflow shown on the next row.
       
       4.Up to 8 rows / 64 buttons in total; configurations beyond 64 are ignored.
         Ini files from older versions (48 buttons max) are automatically completed at startup with the missing
         [Button_49]~[Button_64] sections (hidden by default).

       5.Five languages supported: Simplified Chinese, Traditional Chinese, Japanese, Korean, English.
       
       6.QQ-style window docking to screen edges is supported.
  
       7.Ini file configuration example:
        #############################################
        #[Button_01] - [Button_64] up to 64 buttons supported, beyond 64 ignored
        [Button_XX]
        #Button display name【line breaks supported via </br>, <br>, <br/> or \n; text wider than one line wraps automatically at spaces, and an ellipsis ... is shown when the lines exceed the button height】
        Button_Name=
        #Button visibility【1: visible; anything else: hidden; default: empty = hidden】
        Button_Visible=
        #Button enabled/disabled【1: enabled; anything else: disabled; default: empty = disabled】
        Button_Enable=
        #Button background color【value is an English color name】
        Button_BackColor=
        #Button text color【value is an English color name】
        Button_TextColor=
        #Full path and name of the program to launch
        Program_Name=
        #Launch type【exe: executable file; cmd: cmd command or *.cmd, *.bat file; default: exe】
        Program_Type=
        #Launch arguments
        Program_Param=
        #############################################

        [Button_01]
        Button_Name=Eclipse<br/>v4.21
        Button_Visible=1
        Button_Enable=1
        Button_BackColor=GreenYellow
        Button_TextColor=OrangeRed
        Program_Name=D:\Eclipse_4.21\eclipse.exe
        Program_Type=exe
        Program_Param=-clean

      8.Autostart can be configured from the system tray menu.

      9.Only one instance of this program can run at a time.

      10.Button background and text colors can be customized (see the color names in Sample_ini/Color.html).

      11.Drag & drop to reorder buttons: enable【Edit mode】in the taskbar tray menu【Edit mode】, or
        long-press any button for 3 seconds to enter edit mode;
        press and hold a button, drag it onto the target button and release to move it (iOS-style insertion,
        other buttons shift accordingly); the change applies immediately and is written back to the ini file
        (the order of [Button_XX] sections matches the on-screen order, while comments, blank lines and
        hidden-button settings inside sections are preserved). No restart needed.
        Exit via the tray menu【Edit mode - off】when done, or right-click anywhere inside the window to exit immediately;
        edit mode auto-exits after 8 seconds without
        any order change; clicking buttons in edit mode does not launch programs.
  
      12.High-DPI displays supported: PerMonitorV2 DPI awareness enabled, rendering natively at the
        display's real resolution with no blurring from system bitmap scaling; after switching displays or
        changing the OS scaling ratio, restart the program and the window and buttons adapt to the new
        scaling automatically (requires Windows 10 1703 or later).

      13.Buttons use a rounded keycap-style 3D look (white top face + light gray side wall below);
        pressing sinks and darkens the face, and hovering makes the button text float slightly up and down;
        button text wider than one line wraps automatically at spaces, an ellipsis ... is shown when the lines exceed the button height, and manual line breaks are shown in the ini example of item 7.

      14.Program icon shown automatically at the top-left of each button: the system-associated icon of the program
        configured in Program_Name (exe/cmd/bat/msc/lnk etc.) is extracted and displayed automatically;
        bare commands (e.g. shutdown, notepad) are resolved via PATH; no icon is shown when the program is missing or not configured; icons are re-extracted automatically after ini changes, no restart needed;
        set MainForm_ShowProgramIcon=Off in the [System_Info] section to hide the icons (default On, takes effect automatically after saving). You can also toggle it anytime from the tray [Program Icon] submenu; the change applies instantly and is written back to the ini.

      15.System light/dark theme following supported: set MainForm_Theme=Auto/Light/Dark in the
        [System_Info] section (default Auto follows the system theme; the window follows system theme
        switches in real time without restart); it can also be switched anytime via the tray menu
        【Theme】(Follow System / Light / Dark), applied instantly and saved back to ini; in light mode
        the ini-configured button colors apply unchanged, in dark mode they are auto-converted to
        dark-friendly tones (explicitly configured colors keep their hue); dialogs switch to a dark style in dark mode too.
  
【Changelog】：

      11.v26.12
        Release builds now also copy the project QuickStart.ini to the output as QuickStart_Template.ini (template config; not generated in Debug)
        Release builds now also copy the 5 localized README files and LICENSE to the output Readme folder (not generated in Debug)
        Release builds now also copy the entire project Sample_ini folder to the output (ini configuration examples; not generated in Debug)
        Release builds no longer generate the QuickStart.pdb debug symbols file (Debug unaffected)
        Added program icon at the top-left of each button: system icon of the exe/cmd/bat/msc/lnk configured in ini Program_Name is extracted automatically, bare commands resolved via PATH, no icon when program is missing, ini hot-reload supported
        Added program icon display switch: MainForm_ShowProgramIcon=Off in the [System_Info] ini section hides the icons (default On, hot-reload supported; auto-added on startup when missing from older ini files)
        Added a "Program Icon" submenu (On/Off) to the tray menu; toggling applies instantly and is written back to ini
        Program icons are now extracted asynchronously in the background: the window appears first and icons fade in shortly after, without delaying startup
        Added cross-reload icon cache: extracted icons are reused across ini hot reloads and icon toggling (automatically re-extracted when the program file changes)
        When the [System_Info] section is missing, a complete block identical to the template (System Config comment banner + section + 6 keys in fixed order) is now inserted at the first line of the file on startup, instead of being appended at the end
        Fixed: when [System_Info] is the first line of a UTF-8 BOM ini, settings became unreadable and a duplicate section was appended at the end (a banner comment is now auto-inserted above the section); ANSI/GBK-encoded or read-only ini files are now skipped safely instead of being corrupted or crashing startup
        Added system theme following: ini [System_Info] MainForm_Theme=Auto/Light/Dark (default Auto follows the system light/dark theme; the key is auto-added to old ini files at startup)
        Added a "Theme" submenu to the tray menu (Follow System / Light / Dark); switching applies instantly and is written back to ini
        The title bar follows the theme too (dark title bar in dark mode); in dark mode button and text colors are auto-converted to dark-friendly tones while explicitly configured ini colors keep their hue
        In light mode, ini background/text colors apply unchanged; changing the Theme key supports hot reload
        When the system switches between light and dark themes, running windows follow in real time without restart
        In dark mode, dialogs such as exit confirmation use a dark-styled custom message box; light mode keeps the native system dialog
        The About window follows the light/dark theme as well (window, title bar, button and link all dark-styled in dark mode)
        In dark mode, the tray right-click menu is dark-styled too (including submenus, separators, check marks and arrows); light mode keeps the native menu
        Runtime requirement clarified as .NET Framework 4.7.2 (compile target lowered from 4.8 to 4.7.2, compiler-enforced verification that no newer APIs are used)

      10.v26.11
        Max buttons raised from 48 to 64 (8 rows × 8 columns); older ini files auto-completed with missing sections (hidden by default)
        Buttons shrunk by ~10% (100×66 → 90×59)
        New button style: rounded keycap 3D effect (white face + light gray side wall, no gradient), clear press feedback (sinks and darkens)
        Hovering a button makes its text float slightly up and down
        Manual button-text line breaks: use </br>, <br>, <br/> or \n in ini Button_Name
        Button text wider than one line now wraps automatically at spaces, with a trailing ... when the lines exceed the button height
        Long-press any button for 3 seconds to enter edit mode; edit mode auto-exits after 8 seconds without order changes
        Right-clicking anywhere inside the window now exits edit mode immediately
        Fixed QuickStart.ini not being copied to the build output (csproj Resource changed to Content + PreserveNewest)
        Runtime requirement lowered from .NET Framework 4.8 to 4.7.2 (no 4.8-only APIs used; runs on Win10 LTSC 2019's built-in 4.7.2)
        Ini hot reload: watches the ini file while running and applies external saves within ~1 second (names/visibility/enable/colors/programs/order etc.), no restart needed

      9.v26.10
        Added edit mode, toggled via the tray menu【Edit mode】
        In edit mode, drag to reorder buttons; order is written back to the ini immediately (comments and hidden settings preserved)
        Buttons jitter subtly in edit mode (iOS home-screen edit style), returning to static on exit
        High-DPI display support (PerMonitorV2 DPI awareness), fixing blurry rendering on high-scaling displays
        Max buttons raised from 40 to 48 (6 rows × 8 columns); ini auto-completed with missing sections (hidden by default)
        Buttons slightly shrunk (120×80 → 100×66); window size auto-adapts to the button grid
        Fixed the【Info】window showing a repeated tiled icon (default BackgroundImageLayout Tile); switched to a hi-res optimized framed display
        Fixed window jumping between screens when docking to edges on multi-monitor setups (edge detection now uses the screen the window is on, keeping docking sticky)
        Main window background lightened (light yellow 255,255,225 → 255,255,240)

      8.v22.06
        Added button background and text color settings
        Added button enable/disable settings

      7.v22.05
        Added autostart, configured via the tray menu【Autostart】
        Added multiple-instance check; only one instance can run at a time
        Dropped .Net FrameWork 3.5 support; only .Net FrameWork 4.8 supported
      
      6.v22.03
        Added on/off toggle for edge docking; disabling takes effect immediately (no restart)
        Provided both .Net FrameWork 3.5/4.8 builds for old/new OS and offline environments
        Changed button display order to follow the configuration file instead of [Button_01]~[Button_40]

      5.v22.02
        Added window opacity menu setting, configured via the tray menu【Opacity】
        Restored the window minimize button removed before v21.11

      4.v22.01
        Fixed garbled multi-byte characters on buttons
        Fixed launch errors for programs whose paths contain multi-byte characters
        Added Simplified Chinese, Traditional Chinese, Japanese, Korean and English support, via the tray menu【Language】
        Added a "show" action for when the window is lost, via the tray menu【Show】

      3.v21.12
        Added docking to the top, left and right screen edges (known dual-monitor bug: window oscillates when docking to left/right edges across primary/secondary screens; no workaround yet)
        Added an exit confirmation dialog
        Max buttons raised from 32 to 40
      
      2.v21.11
        Added window opacity setting
        #Main window opacity (0 ~ 100; default 100, 100 means fully opaque)
        MainForm_Opacity=100
      
      1.v21.10
        Initial release

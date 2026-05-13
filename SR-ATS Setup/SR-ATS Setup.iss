#define MyAppName "SR-ATS"
#define MyAppVersion "0.1.1b1"
#define MyAppPublisher "rinnyanneko"
#define MyAppExeName "SR-ATS.exe"

[Setup]
AppId={{8bc058d5-b360-4ff0-b5a7-cf977c3d0534}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=output
OutputBaseFilename=SR-ATS-v{#MyAppVersion}-win-x64-setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
UninstallDisplayName={#MyAppName}
SetupLogging=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
SetupIconFile=assets\favicon_setup.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional icons:"; Flags: unchecked
Name: "simrailconnect"; Description: "Install SimRailConnect integration for SimRail"; GroupDescription: "SimRail integration:"; Flags: checkedonce

[Files]
Source: "payload\SR-ATS\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "payload\SimRailConnect\SimRailConnect.dll"; DestDir: "{app}\components\SimRailConnect"; Flags: ignoreversion
Source: "payload\licenses\*"; DestDir: "{app}\licenses"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\SR-ATS"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\SR-ATS"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Registry]
Root: HKLM; Subkey: "Software\rinnyanneko\SR-ATS"; ValueType: string; ValueName: "InstallDir"; ValueData: "{app}"; Flags: uninsdeletevalue
Root: HKLM; Subkey: "Software\rinnyanneko\SR-ATS"; ValueType: string; ValueName: "SimRailDir"; ValueData: "{code:GetSimRailDirForRegistry}"; Tasks: simrailconnect

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch SR-ATS"; Flags: nowait postinstall skipifsilent

[Code]
var
  SimRailPage: TInputDirWizardPage;
  SimRailDetectedDir: String;

function IsSimRailDirValid(Dir: String): Boolean;
begin
  Result := FileExists(AddBackslash(Dir) + 'SimRail.exe');
end;

function HasMelonLoader(Dir: String): Boolean;
begin
  Result := DirExists(AddBackslash(Dir) + 'MelonLoader');
end;

function IsSimRailRunning(): Boolean;
var
  ResultCode: Integer;
begin
  Exec(
    ExpandConstant('{cmd}'),
    '/C tasklist /FI "IMAGENAME eq SimRail.exe" | find /I "SimRail.exe" > nul',
    '',
    SW_HIDE,
    ewWaitUntilTerminated,
    ResultCode
  );

  Result := ResultCode = 0;
end;

function TryCandidate(Path: String): Boolean;
begin
  Result := IsSimRailDirValid(Path);
  if Result then
    SimRailDetectedDir := Path;
end;

function DetectSimRailDir(): String;
var
  SteamPath: String;
begin
  Result := '';

  if RegQueryStringValue(HKLM32, 'SOFTWARE\Valve\Steam', 'InstallPath', SteamPath) then
  begin
    if TryCandidate(AddBackslash(SteamPath) + 'steamapps\common\SimRail') then
    begin
      Result := SimRailDetectedDir;
      Exit;
    end;
  end;

  if RegQueryStringValue(HKLM64, 'SOFTWARE\Valve\Steam', 'InstallPath', SteamPath) then
  begin
    if TryCandidate(AddBackslash(SteamPath) + 'steamapps\common\SimRail') then
    begin
      Result := SimRailDetectedDir;
      Exit;
    end;
  end;

  if TryCandidate('C:\Program Files (x86)\Steam\steamapps\common\SimRail') then
  begin
    Result := SimRailDetectedDir;
    Exit;
  end;

  if TryCandidate('D:\SteamLibrary\steamapps\common\SimRail') then
  begin
    Result := SimRailDetectedDir;
    Exit;
  end;

  if TryCandidate('E:\SteamLibrary\steamapps\common\SimRail') then
  begin
    Result := SimRailDetectedDir;
    Exit;
  end;

  if TryCandidate('F:\SteamLibrary\steamapps\common\SimRail') then
  begin
    Result := SimRailDetectedDir;
    Exit;
  end;
end;

function GetSelectedSimRailDir(): String;
begin
  if Assigned(SimRailPage) then
    Result := SimRailPage.Values[0]
  else
    Result := '';
end;

function GetSimRailDirForRegistry(Param: String): String;
begin
  Result := GetSelectedSimRailDir();
end;

procedure InitializeWizard();
var
  DefaultSimRailDir: String;
begin
  DefaultSimRailDir := DetectSimRailDir();

  SimRailPage := CreateInputDirPage(
    wpSelectTasks,
    'Select SimRail Installation Folder',
    'Select the folder that contains SimRail.exe.',
    'SR-ATS can install SimRailConnect automatically. Please select your SimRail installation folder.',
    False,
    ''
  );

  SimRailPage.Add('SimRail folder:');

  if DefaultSimRailDir <> '' then
    SimRailPage.Values[0] := DefaultSimRailDir
  else
    SimRailPage.Values[0] := 'C:\Program Files (x86)\Steam\steamapps\common\SimRail';
end;

function ShouldSkipPage(PageID: Integer): Boolean;
begin
  Result := False;

  if Assigned(SimRailPage) and (PageID = SimRailPage.ID) then
  begin
    Result := not WizardIsTaskSelected('simrailconnect');
  end;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  Dir: String;
begin
  Result := True;

  if Assigned(SimRailPage) and (CurPageID = SimRailPage.ID) then
  begin
    Dir := GetSelectedSimRailDir();

    if not IsSimRailDirValid(Dir) then
    begin
      MsgBox(
        'SimRail.exe was not found in this folder.' + #13#10 + #13#10 +
        'Please select the folder that contains SimRail.exe.',
        mbError,
        MB_OK
      );
      Result := False;
      Exit;
    end;

    if not HasMelonLoader(Dir) then
    begin
      MsgBox(
        'MelonLoader was not found in this SimRail installation.' + #13#10 + #13#10 +
        'Please install MelonLoader for SimRail first, then run this installer again.',
        mbError,
        MB_OK
      );
      Result := False;
      Exit;
    end;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  SimRailDir: String;
  ModsDir: String;
  SourceDll: String;
  TargetDll: String;
  BackupDll: String;
begin
  if CurStep = ssPostInstall then
  begin
    if WizardIsTaskSelected('simrailconnect') then
    begin
      SimRailDir := GetSelectedSimRailDir();

      if IsSimRailRunning() then
      begin
        MsgBox(
          'SimRail is currently running.' + #13#10 + #13#10 +
          'Please close SimRail and run this installer again to install or update SimRailConnect.',
          mbError,
          MB_OK
        );
        Exit;
      end;

      ModsDir := AddBackslash(SimRailDir) + 'Mods';
      SourceDll := ExpandConstant('{app}\components\SimRailConnect\SimRailConnect.dll');
      TargetDll := AddBackslash(ModsDir) + 'SimRailConnect.dll';
      BackupDll := TargetDll + '.bak';

      ForceDirectories(ModsDir);

      if FileExists(TargetDll) then
      begin
        FileCopy(TargetDll, BackupDll, False);
      end;

      if not FileCopy(SourceDll, TargetDll, False) then
      begin
        MsgBox(
          'Failed to install SimRailConnect.dll to:' + #13#10 +
          TargetDll,
          mbError,
          MB_OK
        );
      end;
    end;
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  SimRailDir: String;
  TargetDll: String;
  UserChoice: Integer;
begin
  if CurUninstallStep = usUninstall then
  begin
    if RegQueryStringValue(HKLM, 'Software\rinnyanneko\SR-ATS', 'SimRailDir', SimRailDir) then
    begin
      TargetDll := AddBackslash(SimRailDir) + 'Mods\SimRailConnect.dll';

      if FileExists(TargetDll) then
      begin
        UserChoice := MsgBox(
          'Do you also want to remove SimRailConnect from SimRail?' + #13#10 + #13#10 +
          'File to remove:' + #13#10 +
          TargetDll,
          mbConfirmation,
          MB_YESNO
        );

        if UserChoice = IDYES then
        begin
          if IsSimRailRunning() then
          begin
            MsgBox(
              'SimRail is currently running.' + #13#10 + #13#10 +
              'Please close SimRail and run the uninstaller again, or remove this file manually:' + #13#10 +
              TargetDll,
              mbError,
              MB_OK
            );
            Exit;
          end;

          if not DeleteFile(TargetDll) then
          begin
            MsgBox(
              'Failed to remove SimRailConnect.dll from:' + #13#10 +
              TargetDll,
              mbError,
              MB_OK
            );
          end;
        end;
      end;
    end;
  end;
end;
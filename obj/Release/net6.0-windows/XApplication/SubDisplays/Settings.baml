   M S B A M L   `   `   `  ���� D  @GameTimeX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:GameTimeX	GameTimeX      GameTimeX.Settings   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     )localclr-namespace:GameTimeX     -   ���#333333  � 
ResizeMode$  NoResize=� � WindowStyle$
 None=�
����d�$��2q� � Title$ Settings��$	��580��$	��500��$	��450��$	��350�� � WindowStartupLocation$ CenterOwner=�'  !System.Windows.Shell.WindowChrome   WindowChrome      GlassFrameThickness$ 0q�   CornerRadius$ 0��   CaptionHeight$ 0Q�.���  � RowDefinitions �� $��30 ��� $��* �.+��� -   	 ��Name$	 titleBar��$��0��
���   ��� $��0��$��Settings��$��Center=�$
��Left=�$��10,0,0,0q�
��������� $��0��
 �� Orientation$
 
Horizontal=�$��Right=�.+��� -    ��Name$ btnClose��$��X��$��40��$��30��
������ 
������ 
�������   NoMouseOverButtonStyle#��[  � -    �Name$ 
grdContent��$��1��$
��Left=� � ColumnDefinitions �� $	��50* ��� $	��50* � �� $	��35* ��� $	��30* ��� $	��40* �.+��� $��10, 0, 0, 0q�$��1��$��0��$��2��.+��� $��10, 10, 0, 0q�$��2��.+��� $��Create Backup��$
��Bold"�
�������$��0 0 0 5q��� $H��BCreate a backup of the saved profiles and the saved game time data��
�������$��0 0 0 5q�$��12(��� $E��?GameTimeX will be restarted automatically to create the backup!��
���  ��$��0 0 0 5q�$��12(��� $
 
Horizontal=�.+��� -    ��Name$ txtBackupPath��$��0��$��0�����#5c5c5c$��30��$	��300��$��Segoe UI)�$��15(�$��
7, 4, 0, 0q�
������� �� 
IsReadOnly$
 True��$��10 0 0 0q�.���� -   $ btnChooseGameFolder��$��Choose Backup Folder��$��10 0 0 0q�  NormalGuiButton#��[ $	��150���� $
 
Horizontal=�$��	10 10 0 0q�.+��� -   $ btnCreateBackup��$��0��$��0 0 0 0q�$��Create Backup��$	��100��$��30��$
��Left=�${�False��#��[ �� -    ��Name$ cbAutoBackup��  CheckBoxDefaultStyle#��[ $��Auto Backup��$��15 0 0 0q�${�False�� � ToolTip _�  ^� InitialShowDelay$ 0�����#333333$��1q�
����d�
�������.���� `^If checked GameTimeX will create a backap automatically every time the application is started.�� $��10, 10, 0, 0q�$��2��$��0��$��2��.+��� $��10, 10, 0, 0q�$��2��.+��� $��Import Backup��$
��Bold"�
�������$��0 0 0 5q��� $H��BImport a backup of the saved profiles and the saved game time data��
�������$��0 0 0 5q�$��12(��� $E��?GameTimeX will be restarted automatically to import the backup!��
���  ��$��0 0 0 5q�$��12(��� $
 
Horizontal=�.+��� -	   $ txtBackupPathImport��$��0��$��0�����#5c5c5c$��30��$	��300��$��Segoe UI)�$��15(�$��
7, 4, 0, 0q�
�������$
 True��$��10 0 0 0q�.���� -
   $ btnChooseBackUpFile��$��Choose Backup File��$��10 0 0 0q�#��[ $	��150���� -    ��Name$ lblBackUpDate��$��Backup date of file: -��
�������$��10 5 0 0q�$��12(��� -   $ btnImportBackup��$��0��$��0��$��	10 10 0 0q�$��Import Backup��$	��100��$��30��$
��Left=�${�False��#��[ �� $��	20 10 0 0q�$��0��$��0��$��2��$
 Vertical=�.+��� $��0 0 0 10q�
�������$
��Bold"�General Options�� -   $ cbSessionGameTime��$��2��#��[ $4��.Show current game time session when monitoring�� _� $ 0�����#333333$��1q�
����d�
�������.���� ZXIf checked the current session game time is shown below the start/stop monitoring button�� $��	0 20 0 10q�
�������$
��Bold"�Key for game monitoring�� -   $ cbMonitoringKeyActive��#��[ $��
Key active���� -   $ lblCurrentKey��$��0 5 0 0q�
�������Current key: (no key)�� -   $ btnMonitoringKey��$��0��$��0 10 0 0q�$��Monitor key��$	��100��$��30��$
��Left=�${�False��#��[ �� $��2��$��2��$��Right=�$��Bottom=�
�������$��0, 0, 0, 10q�Version 1.7.1
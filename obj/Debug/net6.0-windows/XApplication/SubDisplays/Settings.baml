   M S B A M L   `   `   `  ����D  @GameTimeX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:GameTimeX	GameTimeX  5   	       GameTimeX.Settings   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:GameTimeX     5   	   -   5        ���#333333  � 
ResizeMode$  NoResize=�5	   	      DefaultWindowStyle#��[  5
   	    � WindowStyle$
 None=�5   	   
����d�$��2q�6j    � Title$ Settings��5   	   $	��700��6   $	��500��6+   $	��450��67   $	��350��6G    � WindowStartupLocation$ CenterOwner=�6V   '  !System.Windows.Shell.WindowChrome   WindowChrome 5        5   
      GlassFrameThickness$ 0q�6      CornerRadius$ 0��6/      CaptionHeight$ 0Q�6@   .��5      �  � RowDefinitions 5   
   �� 5      $��30 �6   5      �� 5      $��* �6   5      .+�5   
   �� -   5        	 ��Name$	 titleBar��5   .   $��0��6   
���   �6B   �� 5   
   $��0��6   $��Settings��6!   $��Center=�61   $
��Left=�6L   $��10,0,0,0q�6g   
�������5      �� 5   
   $��0��6   
 �� Orientation$
 
Horizontal=�6"   $��Right=�6;   .+�5       �� -   5         ��Name$ btnClose��5       $��X��6'   $��40��63   $��30��6>   
������ 
������ 
�������  NoMouseOverButtonStyle#��[ 5#   6   5$      5%      � 5'   
   -   5         �Name$ 
grdContent��5'      $��1��6   $
��Left=�60    � ColumnDefinitions 5(      �� 5)      $	��50* �6#   �� 5*      $	��50* �6#    5,      �� 5-      $	��35* �6    6/   �� 5.      $	��30* �6    6/   �� 5/      $	��40* �6    6/   .+�51      �� $��10, 20, 0, 0q�6   $��1��6/   $��0��6<   $��2��6L   .+�52      �� $��10, 10, 0, 0q�6   $��2��64   .+�53      �� $��Create Backup��6    $
��Bold"�65   
�������$��0 0 0 5q�6Z   �� 54      $H��BCreate a backup of the saved profiles and the saved game time data��6    
�������$��0 0 0 5q�6}   $��12(�55       �� 56      $E��?GameTimeX will be restarted automatically to create the backup!��6    
���  ��$��0 0 0 5q�6x   $��12(�57       58      �� 59      $
 
Horizontal=�6   .+�5:      �� -   5         ��Name$ txtBackupPath��5:   ;   $��0��6   $��0��6+   ���#5c5c5c$��30��5;      $	��300��6&   $��15(�62   $��
7, 4, 0, 0q�6@   
������� �� 
IsReadOnly$
 True��5<      $��10 0 0 0q�6,   .��6>   5=      �� 5>      -   5        $ btnChooseGameFolder��5>      $��Choose Backup Folder��6:   $��10 0 0 0q�6Y     NormalGuiButton#��[ 6k   $	��150��6�   5?      5@      �� 5A      $
 
Horizontal=�6   $��	10 10 0 0q�66   .+�5B      �� -   5        $ btnCreateBackup��5B      $��0��66   $��0 0 0 0q�6F   $��Create Backup��6W   $	��100��5C      $��30��6)   $
��Left=�65   ${�False��5D      #��[ 5E      5F      �� 5G      -   5         ��Name$ cbAutoBackup��5I        CheckBoxDefaultStyle#��[ 5G      $��Auto Backup��5H      $��15 0 0 0q�65   ${�False��5J       � ToolTip 5L      _� 5M       ^� InitialShowDelay$ 0��6&   ���#333333$��1q�6_   
����d�
�������.��5N   "   �� `^If checked GameTimeX will create a backap automatically every time the application is started.5P   #   5Q      5S      5T      5V      �� 5W      $��10, 10, 0, 0q�6   $��2��6/   $��0��6<   $��2��6L   .+�5X      �� $��10, 10, 0, 0q�6   $��2��64   .+�5Y      �� $��Import Backup��6    $
��Bold"�65   
�������$��0 0 0 5q�6Z   �� 5Z      $H��BImport a backup of the saved profiles and the saved game time data��6    
�������$��0 0 0 5q�6}   $��12(�5[   $   �� 5\      $E��?GameTimeX will be restarted automatically to import the backup!��6    
���  ��$��0 0 0 5q�6x   $��12(�5]   $   5^      �� 5_      $
 
Horizontal=�6   .+�5`      �� -	   5        $ txtBackupPathImport��5`   ;   $��0��6   $��0��6+   ���#5c5c5c$��30��5a   %   $	��300��61   $��15(�6=   $��
7, 4, 0, 0q�6K   
�������$
 True��5b   %   $��10 0 0 0q�67   .��6I   5c      �� 5d      -
   5        $ btnChooseBackUpFile��5d      $��Choose Backup File��6:   $��10 0 0 0q�6W   #��[ 6i   $	��150��6�   5e   %   5f      �� 5g      -   5         ��Name$ lblBackUpDate��5g      $��Backup date of file: -��65   
�������$��10 5 0 0q�6f   $��12(�5h      �� 5i      -   5        $ btnImportBackup��5i      $��0��63   $��0��6A   $��	10 10 0 0q�6Q   $��Import Backup��6d   $	��100��5j      $��30��6%   $
��Left=�61   ${�False��5k      #��[ 5l      5m      5n      �� 5o      $��	20 10 0 0q�6   $��0��6,   $��0��69   $��2��6I   $
 Vertical=�6]   .+�5p      �� $��0 0 0 10q�6   
�������$
��Bold"�6A   General Options5r      �� 5s      -   5        $ cbSessionGameTime��5u      $��2��5s      #��[ 6(   $4��.Show current game time session when monitoring��5t       5v      _� 5w      $ 0��6"   ���#333333$��1q�6[   
����d�
�������.��5x      �� ZXIf checked the current session game time is shown below the start/stop monitoring button5z      5{      5}      �� 5~      -   5        $ cbGameSwitcher��5�      $��2��5~      #��[ 6(   $/��)Automatic switching between game profiles��5      $��0 10 0 0q�5�       5�      _� 5�      $ 0��6"   ���#333333$��1q�6[   
����d�
�������.��5�      �� $
��Wrap=�6(   B@When enabled, GameTimeX automatically switches the game profile.�� 5�   c   ><Note: The game folder path must be specified in the profile!5�      5�      5�      �� 5�      -   5        $ btnShowDataFolder��5�      $��3��64   $��0 10 0 0q�6D   $��Show data folder��6V   $	��100��5�      $��30��6%   $
��Left=�61   $
{�True��5�      #��[ 5�      5�      �� 5�      $��	0 10 0 10q�6   
�������$
��Bold"�6B   Key for game monitoring5�      �� 5�      $
 
Horizontal=�6   .+�5�      �� -   5        $ cbMonitoringKeyActive��5�   !   #��[ 5�      $��
Key active��5�   !   5�      �� 5�      -   5        $ cbShowToastNotification��5�   !   #��[ 5�      $��Show toast notification��5�   !   $��10 0 0 0q�5�   !   ${�False��5�   !    5�      _� 5�      $ 0��6&   ���#333333$��1q�6_   
����d�
�������.��5�   "   �� $
��Wrap=�6,   ��When enabled, a small window will appear in the upper-right corner of the screen for 3 seconds, indicating that gameplay recording has started or stopped.5�   #   5�      5�      5�      �� 5�      -   5        $ lblCurrentKey��5�   @   $��0 5 0 0q�6   
�������Current key: (no key)5�      �� 5�      -   5        $ btnMonitoringKey��5�      $��0��63   $��0 10 0 0q�6C   $��Monitor key��6U   $	��100��5�      $��30��6%   $
��Left=�61   ${�False��5�      #��[ 5�      5�      5�      �� 5�      $��2��6   $��2��6%   $��Right=�5�      $��Bottom=�5�      
�������$��0, 0, 0, 10q�5�      Version 2.0.15�      5�      5�      5�      
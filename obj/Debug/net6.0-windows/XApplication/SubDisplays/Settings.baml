   M S B A M L   `   `   `  ����D  @GameTimeX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:GameTimeX	GameTimeX  5   	       GameTimeX.Settings   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:GameTimeX     5   	   -   5        ���#333333  � 
ResizeMode$  NoResize=�5	   	    � WindowStyle$
 None=�5
   	   
����d�$��2q�6j    � Title$ Settings��5   	   $	��450��6   $	��500��6+   $	��450��67   $	��350��6G    � WindowStartupLocation$ CenterOwner=�6V   '  !System.Windows.Shell.WindowChrome   WindowChrome 5        5   
      GlassFrameThickness$ 0q�6      CornerRadius$ 0��6/      CaptionHeight$ 0Q�6@   .��5      �  � RowDefinitions 5   
   �� 5      $��30 �6   5      �� 5      $��* �6   5      .+�5   
   �� -   5        	 ��Name$	 titleBar��5   .   $��0��6   
���   �6B   �� 5   
   $��0��6   $��Settings��6!   $��Center=�61   $
��Left=�6L   $��10,0,0,0q�6g   
�������5      �� 5   
   $��0��6   
 �� Orientation$
 
Horizontal=�6"   $��Right=�6;   .+�5      �� -   5         ��Name$ btnClose��5      $��X��6'   $��40��63   $��30��6>   
������ 
������ 
�������   NoMouseOverButtonStyle#��[  5"   6   5#      5$      � 5&   
   -   5         �Name$ 
grdContent��5&      $��1��6   $
��Left=�60    � ColumnDefinitions 5'      �� 5(      $	��50* �6#   �� 5)      $	��50* �6#    5+      �� 5,      $	��50* �6    6/   �� 5-      $	��50* �6    6/   .+�5/      �� $��10, 10, 0, 0q�6   $��0��6/   $��0��6<   $��2��6L   .+�50      �� $��10, 10, 0, 0q�6   $��2��64   .+�51      �� $��Create Backup��6    $
��Bold"�65   
�������$��0 0 0 5q�6Z   �� 52      $H��BCreate a backup of the saved profiles and the saved game time data��6    
�������$��0 0 0 5q�6}   $��12(�53       �� 54      $E��?GameTimeX will be restarted automatically to create the backup!��6    
���  ��$��0 0 0 5q�6x   $��12(�55       56      �� 57      $
 
Horizontal=�6   .+�58      �� -   5         ��Name$ txtBackupPath��58   ;   $��0��6   $��0��6+   ���#5c5c5c$��30��59      $	��300��6&   $��Segoe UI)�63   $��15(�6I   $��
7, 4, 0, 0q�6W   
������� �� 
IsReadOnly$
 True��5:      $��10 0 0 0q�6,   .��6>   5;      �� 5<      -   5        $ btnChooseGameFolder��5<      $��Choose Backup Folder��6:   $��10 0 0 0q�6Y     NormalGuiButton#��[ 6k   $	��150��6�   5=      5>      �� 5?      -   5        $ btnCreateBackup��5?      $��0��63   $��0��6A   $��	10 10 0 0q�6Q   $��Create Backup��6d   $	��100��5@      $��30��6!   $
��Left=�6-   ${�False��5A      #��[ 5B      5C      5D      �� 5E      $��10, 10, 0, 0q�6   $��1��6/   $��0��6<   $��2��6L   .+�5F      �� $��10, 10, 0, 0q�6   $��2��64   .+�5G      �� $��Import Backup��6    $
��Bold"�65   
�������$��0 0 0 5q�6Z   �� 5H      $H��BImport a backup of the saved profiles and the saved game time data��6    
�������$��0 0 0 5q�6}   $��12(�5I   $   �� 5J      $E��?GameTimeX will be restarted automatically to import the backup!��6    
���  ��$��0 0 0 5q�6x   $��12(�5K   $   5L      �� 5M      $
 
Horizontal=�6   .+�5N      �� -   5        $ txtBackupPathImport��5N   ;   $��0��6   $��0��6+   ���#5c5c5c$��30��5O   %   $	��300��61   $��Segoe UI)�6>   $��15(�6T   $��
7, 4, 0, 0q�6b   
�������$
 True��5P   %   $��10 0 0 0q�67   .��6I   5Q      �� 5R      -	   5        $ btnChooseBackUpFile��5R      $��Choose Backup File��6:   $��10 0 0 0q�6W   #��[ 6i   $	��150��6�   5S   %   5T      �� 5U      -
   5         ��Name$ lblBackUpDate��5U      $��Backup date of file: -��65   
�������$��10 5 0 0q�6f   $��12(�5V      �� 5W      -   5        $ btnImportBackup��5W      $��0��63   $��0��6A   $��	10 10 0 0q�6Q   $��Import Backup��6d   $	��100��5X      $��30��6%   $
��Left=�61   ${�False��5Y      #��[ 5Z      5[      5\      5]      5^      5_      
   M S B A M L   `   `   `  ����D  @GameTimeX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:GameTimeX	GameTimeX  5   	       GameTimeX.Rename   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:GameTimeX     5   	   -   5          � Title$  Create New Profile��5   	   $	��150��6$   $	��300��61   ���#333333   DefaultWindowStyle#��[  5	   	    � WindowStyle$
 None=�5
   	    � WindowStartupLocation$ CenterOwner=�6M   
����d�$��2q�6�   '  !System.Windows.Shell.WindowChrome   WindowChrome 5        5   
      GlassFrameThickness$ 0q�6      CornerRadius$ 0��6/      CaptionHeight$ 0Q�6@   .��5      �  � RowDefinitions 5   
   �� 5      $��30 �6   5      �� 5      $��* �6   5      .+�5   
   �� -   5         ��Name$ titleBar��5   .   $��0��6   
���   �6B   �� 5   
   $��0��6   $��Rename Profile��6!   $��Center=�67   $
��Left=�6R   $��10,0,0,0q�6m   
�������5      �� 5   
   $��0��6   	 �� Orientation$	 
Horizontal=�6"   $��Right=�6;   .+�5      �� -   5        
 ��Name$
 btnClose��5      $��X��6'   $��40��63   $��30��6>   
������ 
������ 
�������  NoMouseOverButtonStyle#��[ 5!   6   5"      5#      � 5&   
   -   5         �Name$ 
grdContent��5&      $��1��6    � ColumnDefinitions 5'      �� 5(      $	��70* �6#   �� 5)      $	��30* �6#    5+      �� 5,      $	��50* �6    6/   .+�5/      �� -   5         ��Name$ txtProfileName��5/   3   $��0��6   $��0��6#   ���#5c5c5c$��30��50      $��15(�6&   $��
7, 4, 0, 0q�64   
�������$��10, 0, 0, 0q�6\   .��6q   51      �� 53      -   5        $
 	btnRename��53   3   $��0��6   $��1��6#   $
��save��6F   $��70��6U   $��30��6`   
���   ����#0099ff
�������  NormalGuiButton#��[ 56      57      59      5:      5;      
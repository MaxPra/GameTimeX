   M S B A M L   `   `   `  ����D  @GameTimeX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:GameTimeX	GameTimeX  5   	       GameTimeX.ImageCropper   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:GameTimeX     5   	   -   5          �Name$  imageCropperWindow��5	      ���#333333 � 
ResizeMode$ NoResize=�6	      DefaultWindowStyle#��[  5
   	    � WindowStyle$
 None=�5   	   
����d�$��2q�69    � Title$ Settings��5   	   $	��500��6   $	��800��6+   $	��500��67   $	��800��6G    � WindowStartupLocation$ CenterOwner=�6V   '  !System.Windows.Shell.WindowChrome   WindowChrome 5        5   
      GlassFrameThickness$ 0q�6      CornerRadius$ 0��6/      CaptionHeight$ 0Q�6@   .��5      � 	 � RowDefinitions	 5   
   �� 5      $��30 �6   5      �� 5      $��* �6   5      .+�5   
   �� -   5        
 ��Name$
 titleBar��5   M   $��0��63   
���   �6a   �� 5   
   $��0��6   $��Crop Profile Image��6!   $��Center=�6;   $
��Left=�6V   $��10,0,0,0q�6q   
�������5      �� 5   
   $��0��6    �� Orientation$ 
Horizontal=�6"   $��Right=�6;   .+�5       �� -   5         ��Name$ btnClose��5       $��X��6'   $��40��63   $��30��6>   
������ 
������ 
�������  NoMouseOverButtonStyle#��[ 5#   6   5$      5%      � 5'   
   -   5         �Name$ 
grdContent��5'      $��1��6   $
��Left=�60   .+�5(      �� -   5         ��Name$ imgProfilePic��5(      5)      �� 5*      -   5         ��Name$ 
cvsCropper��5*      
������ .+�50      �� -   5        $
 recTransformArea��50      
������ 
�������$��2��53      $	��300��54      $	��300��55       �� Left$ 0��56      
 �� Top$ 0��57      58      59      5:      5;      5<      
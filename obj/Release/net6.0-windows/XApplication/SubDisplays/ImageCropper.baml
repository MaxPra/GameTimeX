   M S B A M L   `   `   `  ���� D  @GameTimeX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:GameTimeX	GameTimeX      GameTimeX.ImageCropper   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     )localclr-namespace:GameTimeX     -     �Name$  imageCropperWindow�����#333333 � 
ResizeMode$ NoResize=� � WindowStyle$
 None=�
����d�$��2q� � Title$ Settings��$	��500��$	��800��$	��500��$	��800�� � WindowStartupLocation$ CenterOwner=�'  !System.Windows.Shell.WindowChrome   WindowChrome      GlassFrameThickness$ 0q�   CornerRadius$ 0��   CaptionHeight$ 0Q�.��� 	 � RowDefinitions	 �� $��30 ��� $��* �.+��� -   
 ��Name$
 titleBar��$��0��
���   ��� $��0��$��Crop Profile Image��$��Center=�$
��Left=�$��10,0,0,0q�
��������� $��0�� �� Orientation$ 
Horizontal=�$��Right=�.+��� -    ��Name$ btnClose��$��X��$��40��$��30��
������ 
������ 
�������   NoMouseOverButtonStyle#��[  � -    �Name$ 
grdContent��$��1��$
��Left=�.+��� -    ��Name$ imgProfilePic���� -    ��Name$ 
cvsCropper��
������ .+��� -   $
 recTransformArea��
������ 
�������$��2��$	��300��$	��300�� �� Left$ 0��
 �� Top$ 0��
   M S B A M L   `   `   `  ����P  LCustomizable Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:SGLauncher2._0.WindowsSGLauncher2._0.Windows  5   	   *    $SGLauncher2._0.Windows.window_update   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   6local$clr-namespace:SGLauncher2._0.Windows     5   	   S OHandyControl, Version=3.4.0.0, Culture=neutral, PublicKeyToken=45be8712787a1e5b0hc'https://handyorg.github.io/handycontrol  5	   	   ] YFontAwesome6.Fonts.Net, Version=2.5.1.0, Culture=neutral, PublicKeyToken=150287d219411f043fa*http://schemas.fontawesome.com/icons/fonts  5
   	   -   5          � Title$  window_update��5   	   $	��550��6   $
��1000��6,    � WindowStartupLocation$ CenterScreen=�5   	    � WindowStyle$
 None=�5   	   
������  � AllowsTransparency .  � 	Resources 5       �� 
TargetType �� Property �� Value��%d   '��       �� 5   
   " ��6   .�5      ��  � ��$ /Fonts/#Noto Sans KR)�6+   5      5      .��5      �� -   5         ��Name$ border_main��5      ���#fff	 �� CornerRadius$	 5��6Z   ._�5   
   � .+�5      �� $
��Bold"�6   $��15q�6*   $��20(�66   $  FontAwesome6.Fonts.FontAwesome  5      
   Icon$
 Solid_ClockRotateLeft=�6!   $��0,0,10,0q�6>   업데이트 센터�� 6Z   �� 5      $��16(�6   $��Medium"�6$   .�68   .,모드팩의 업데이트를 확인하세요6J   5      �� 5      -   5        ���#fff
������ $��35��5   Y   $��35��6e   $	��Top=�6p   $��Right=�6�   $��10q�6�   $��Center=�6�   ��5       �� 5!       �� Color$
 #000��6&    �� Opacity$	 0.6Q�63   .��5#        $
 Solid_Xmark=�6!   $��20(�64   5$      �� 5&      $��Center=�6   $
��15,0q�64   .+�5'      � $	��150��6    � ColumnDefinitions 5(      �� 5)      $	��400 �6+   �� 5*      .+�5-      �� $��0��6!   $
��15,0q�61   $��Center=�6?   .+�5.      �� -   5        $��textblock_modpack��5.   $   $��/Fonts/#Montserrat-Alt1)�6=   $��26(�6b   
��� E��SGAYRIM 3.050      �� 51      -   5        $��text_modpack_current��51   $   $��14(�6@   $��0,0,0,0q�6N   설치된 버전 : 1.0.053      �� 54      -   5         ��Name$ text_modpack_latest��54   $   $��14(�6A   최신 버전 : 1.0.056      57      �� 59      $��Center=�6!   $
��15,0q�6<   $��1��6J   .+�5:      �� -   5        $��Stretch=�5:   @      ButtonPrimary#��[  6^   $��/Fonts/#Noto Sans KR)�6�   $
��Bold"�6�   $��75��6�   ��5;      �� 5<   "     5=   &   $
 Solid_PaperPlane=�65   $��0,0,5,0q�6M   *( 업데이트 배포 페이지로 이동5>   #   5A      5B      � 5D      -   5        $��modpack_blocker��5D      $��1��62   ���#fff.+�5E      �� $��Center=�6$   $��Center=�6?   $
��Bold"�6\   $��16(�6n   
��� � �  5F      $
 Solid_CircleCheck=�6,   ., 이미 모드팩이 최신버전 입니다.5G      5H      5L      5M      5N      5O      5P      
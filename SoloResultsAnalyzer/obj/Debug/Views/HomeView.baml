   M S B A M L   `   `   `  ����N  JSoloResultsAnalyzer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:SoloResultsAnalyzer.ViewsSoloResultsAnalyzer.Views  5      (    "SoloResultsAnalyzer.Views.HomeView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      9local'clr-namespace:SoloResultsAnalyzer.Views     5        � 	Resources  5	         listHeaderStyle �� 
TargetType �� Property �� Value��%Y   &	        �� 5
   
   " ��6(   .�5      ��  � y�$ 	Collapsed=�6+   5      5      .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $��4* �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��2* �6   .+�5   
   �� $��0��6   $��0��6   $��2��6-   $��STL Solo Results Analyzer��5      $��48(�64   $��Center=�6B   $��Center=�6]   � 5   
   $��1��6   $��0��6!    � Orientation$ Vertical=�61   $	��Top=�6H   $��Center=�6`   .+�5      �� $��Import Runs��6   $��20q�6F   $��5q�6R   ��6+   �� *Import+6R   �� 5      $��Create Event Reports��6   $��20q�6T   $��5q�6`   ��64   �� *EventReport+6`   �� 5      $��Update Championships��6   $��20q�6[   $��5q�6g   ��64   �� *ChampionshipReport+6g   �� 5       $��View Drivers��6   $��20q�6L   $��5q�6X   ��6,   �� *ViewDrivers+6X   �� 5!      $��Edit Season��6   $��20q�6J   $��5q�6V   ��6+   �� *
EditSeason+6V   �� 5"      $��
New Season��6   $��20q�6H   $��5q�6T   ��6*   �� *	NewSeason+6T   5#      � 5%   
   $��1��6   $��1��6!   $ Vertical=�61   $	��Top=�6H   $��Stretch=�6`   .+�5&      ��  �� ContentStringFormat$  {0} Season��6G   $��36(�6i   ��6   �� *
SeasonYear+ �� FallbackValue$
 2000��6i   �� 5'      
������ $��0q�6O   
���   ���6   �� *Events+��5(      �� 5)      " ��6   .�5*      ��  � ��
 ���� 67   �� 5+       � �� 5,      �� 5-   "   	 �� 
TargetType"	 ��62   .R�5.   &   �� 
������ $��0q�6G   #��z��6[   ._�5/   *   �� $��Stretch=�6?   #��z��6]   $
��Auto��6�   $��0q�6�   
 �� Content#
 z��6�   50   '   51   #   53      54       �� View 56      �� 57      ! �� ColumnHeaderContainerStyle# [  6   .P�58      �� $��Event Number��6)    �� DisplayMemberBinding 6?   �� *EventNumber+ �� StringFormat$ 	Event {0}���� 59      $
��Date��6)    67   �� *Date+$ D���� 5:      $��Location��6)    6;   �� *
Location+5;      5>      5?      5A      5B      
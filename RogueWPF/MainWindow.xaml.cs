using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using CSRogue.GameControl;
using CSRogue.Map_Generation;
using Malison.WinForms;
using RogueWPF.Mapping;
using RogueWPF.UI;

namespace RogueWPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private readonly MapHandler _mapHandler;
		private readonly Game _game =new Game();
		private readonly KeyboardHandler _keyboard;

		public MainWindow()
		{
			InitializeComponent();
			TerminalCtl.GlyphSheet = GlyphSheet.Terminal10x12;
			_mapHandler = new MapHandler(TerminalCtl, MapHost, _game);
			_keyboard = new KeyboardHandler(_game);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			_mapHandler.InitializeMap();
		}

		private void MapHost_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
		{
			_mapHandler.Resize();
		}

		private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			_keyboard.DispatchOnKey(e.Key);
		}

	}
}

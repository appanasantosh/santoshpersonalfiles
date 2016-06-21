
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Diagnostics;
using PMImportImplementation;

namespace PMDataMigration
{
	/// <summary>
	/// Class with program entry point.
	/// </summary>
	internal sealed class Program
	{
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			try 
			{
				PMMigrationLogger.Configure();
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				
				var catalog = new DirectoryCatalog(".","*");
				CompositionContainer container = new CompositionContainer(catalog);
				
				MainForm fmMain = new MainForm();
				container.ComposeParts(fmMain);
				
				Application.Run(fmMain);
			} 
			catch (CompositionException ex)
			{
				MessageBox.Show(ex.ToString());
				//throw;
			}
			
		}
		
	}
}

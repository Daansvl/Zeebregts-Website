using System;
using System.Windows.Controls;

namespace eBrochure_zeebregts
{
	public interface IProduct
	{
		void SetPrijs(Double p);
		double GetPrijs();
		RekenUnit GetRekenUnit();
		double GetVerpakkingToeslag();
		void SetVerpakkingToeslag(double t);
		void SetKleur(string s);
		string GetKleur();
		void SetProductID(string s);
		string GetProductID();
		void SetIMG(Image b);
		Image GetIMG();
		void SetNaam(string s);
		string GetNaam();

	}
	public class RekenUnit
	{

	}
}

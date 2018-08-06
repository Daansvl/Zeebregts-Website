
namespace eBrochure_zeebregts
{
	public abstract class IOfferteTracker
	{
		public BaseOfferte offerteBasis_
		{ get; set; }

		public KavelOfferte offerteKavel_
		{ get; set; }
		public RuimteOfferte offerteRuimte_
		{ get; set; }
		protected virtual IOfferte BouwOfferte()
		{
			return new BaseOfferte();
		}
		public virtual void NieuwOfferte()
		{
			 BouwOfferte();
		}
	}
}

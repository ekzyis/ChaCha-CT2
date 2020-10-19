using System.Windows.Controls;

namespace Cryptool.Plugins.ChaCha
{
    partial class ChaChaPresentation
    {
        public void InitUserKeystreamBlock(ulong keyblockNr)
        {
            _chacha.ExecuteChaChaWithUserKeystreamBlock(keyblockNr);
        }
    }

    class UserKeystreamBlockGenPage : KeystreamBlockGenPage
    {
        public UserKeystreamBlockGenPage(ContentControl pageElement, ChaChaPresentation pres_, ulong keyblockNr_) : base(pageElement, pres_, keyblockNr_) { }

        protected override void Init()
        {
            pres.InitUserKeystreamBlock(keyBlockNr);
            base.Init();
        }

        public static ResultType<uint> MapResultType(ResultType<uint> resultType)
        {
            return ResultType.GetUserType(resultType);
        }

        public static ResultType<uint[]> MapResultType(ResultType<uint[]> resultType)
        {
            return ResultType.GetUserType(resultType);
        }

        protected override int MapIndex(ResultType<uint[]> resultType, int i)
        {
            // offset is always zero because intermediate values of UserKeystreamBlockGenPage instances do not share their values with other pages.
            return i;
        }
        protected override int MapIndex(ResultType<uint> resultType, int i)
        {
            // offset is always zero because intermediate values of UserKeystreamBlockGenPage instances do not share their values with other pages.
            return i;
        }
        protected override uint[] GetMappedResult(ResultType<uint[]> resultType, int index)
        {
            return pres.GetResult(MapResultType(resultType), MapIndex(resultType, index));
        }

        protected override uint GetMappedResult(ResultType<uint> resultType, int index)
        {
            return pres.GetResult(MapResultType(resultType), MapIndex(resultType, index));
        }
        protected override string GetMappedHexResult(ResultType<uint[]> resultType, int i, int j)
        {
            return pres.GetHexResult(MapResultType(resultType), MapIndex(resultType, i), j);
        }
        protected override string GetMappedHexResult(ResultType<uint> resultType, int index)
        {
            return pres.GetHexResult(MapResultType(resultType), MapIndex(resultType, index));
        }
    }

    partial class Page
    {
        public static UserKeystreamBlockGenPage UserKeystreamBlockGenPage(ChaChaPresentation pres, ulong keyBlockNr)
        {
            // using static function as factory to hide the name assigned to the KeystreamBlockGenPage ContentControl element in the XAML code
            return new UserKeystreamBlockGenPage(pres.UIKeystreamBlockGenPage, pres, keyBlockNr);
        }
    }
}

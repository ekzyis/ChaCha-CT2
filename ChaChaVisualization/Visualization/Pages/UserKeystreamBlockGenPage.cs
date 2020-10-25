using System.Diagnostics;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaChaVisualization
{
    partial class ChaChaPresentation
    {
        public void InitUserKeystreamBlock(ulong keyblockNr)
        {
            _chacha.ExecuteChaChaWithUserKeystreamBlock(keyblockNr);
        }

        public void InitDiffusionResultsForUserKeystreamBlock(ulong keyblockNr)
        {
            _chacha.ExecuteChaChaWithDiffusionKeyAndUserKeystreamBlock(_diffusionKey, keyblockNr);
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

        protected override void InitDiffusionResults()
        {
            pres.InitDiffusionResultsForUserKeystreamBlock(keyBlockNr);
        }

        public static ResultType<uint> MapResultType(ResultType<uint> resultType)
        {
            // don't map diffusion types
            if (!resultType.Name.StartsWith("FLIPPED_BITS") && !resultType.Name.EndsWith("DIFFUSION")) return ResultType.GetUserType(resultType);
            else return resultType;
        }

        public static ResultType<uint[]> MapResultType(ResultType<uint[]> resultType)
        {
            // don't map diffusion types
            if (!resultType.Name.StartsWith("FLIPPED_BITS") && !resultType.Name.EndsWith("DIFFUSION")) return ResultType.GetUserType(resultType);
            else return resultType;
        }

        protected override int MapIndex(ResultType<uint[]> resultType, int i)
        {
            switch (resultType.Name)
            {
                case "USER_CHACHA_HASH_ORIGINAL_STATE":
                case "USER_CHACHA_HASH_ADD_ORIGINAL_STATE":
                case "USER_CHACHA_HASH_LITTLEENDIAN_STATE":
                case "CHACHA_HASH_ORIGINAL_STATE_DIFFUSION":
                case "CHACHA_HASH_ADD_ORIGINAL_STATE_DIFFUSION":
                case "CHACHA_HASH_LITTLEENDIAN_STATE_DIFFUSION":
                    // These types are called with the keystream block as index i. Return zero because user keystream block is always at index 0 since there are no other keystream blocks (they overwrite each other).
                    return 0;
                default:
                    // offset is always zero because intermediate values of UserKeystreamBlockGenPage instances do not share their values with other pages.
                    return i;
            }
        }
        protected override int MapIndex(ResultType<uint> resultType, int i)
        {
            switch(resultType.Name)
            {
                case "FLIPPED_BITS_ORIGINAL_STATE":
                case "FLIPPED_BITS_ADD_ORIGINAL_STATE":
                case "FLIPPED_BITS_LITTLEENDIAN_STATE":
                    // These types are called with the keystream block as index i. Return zero because user keystream block is always at index 0 since there are no other keystream blocks (they overwrite each other).
                    return 0;
                default:
                    // offset is always zero because intermediate values of UserKeystreamBlockGenPage instances do not share their values with other pages.
                    return i;
            }
        }
        protected override uint[] GetMappedResult(ResultType<uint[]> resultType, int index)
        {
            resultType = MapResultType(resultType);
            return pres.GetResult(resultType, MapIndex(resultType, index));
        }

        protected override uint GetMappedResult(ResultType<uint> resultType, int index)
        {
            resultType = MapResultType(resultType);
            return pres.GetResult(resultType, MapIndex(resultType, index));
        }
        protected override string GetMappedHexResult(ResultType<uint[]> resultType, int i, int j)
        {
            resultType = MapResultType(resultType);
            return pres.GetHexResult(resultType, MapIndex(resultType, i), j);
        }
        protected override string GetMappedHexResult(ResultType<uint> resultType, int index)
        {
            resultType = MapResultType(resultType);
            return pres.GetHexResult(resultType, MapIndex(resultType, index));
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

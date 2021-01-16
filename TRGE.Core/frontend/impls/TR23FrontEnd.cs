namespace TRGE.Core
{
    internal class TR23FrontEnd : AbstractTRFrontEnd
    {
        internal override bool HasFMV
        {
            get => HasActiveOperation(TR23OpDefs.FMV);
            set => SetOperationActive(TR23OpDefs.FMV, value);
        }

        protected override TROpDef GetOpDefFor(ushort scriptData)
        {
            return TR23OpDefs.Get(scriptData);
        }
    }
}
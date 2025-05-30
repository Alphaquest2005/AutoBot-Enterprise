using Asycuda421; // Assuming Value_declaration_formIdentification_segmentDeclarant_segment is here
using System;
using ValuationDS.Business.Entities; // Assuming xC71_Declarant_segment is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        private void ExportDeclarant(Value_declaration_formIdentification_segmentDeclarant_segment aDecl, xC71_Declarant_segment dDecl)
        {
            try
            {
                // Ensure Text collections are initialized if null
                if (aDecl.Address.Text == null) aDecl.Address.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (aDecl.Code.Text == null) aDecl.Code.Text = new System.Collections.ObjectModel.ObservableCollection<string>();
                if (aDecl.Name.Text == null) aDecl.Name.Text = new System.Collections.ObjectModel.ObservableCollection<string>();

                aDecl.Address.Text.Add(dDecl.Address); // Potential NullReferenceException if dDecl is null
                aDecl.Code.Text.Add(dDecl.Code); // Potential NullReferenceException if dDecl is null
                aDecl.Name.Text.Add(dDecl.Name); // Potential NullReferenceException if dDecl is null
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
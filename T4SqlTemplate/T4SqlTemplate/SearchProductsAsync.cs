﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン: 17.0.0.0
//  
//     このファイルへの変更は、正しくない動作の原因になる可能性があり、
//     コードが再生成されると失われます。
// </auto-generated>
// ------------------------------------------------------------------------------
namespace T4SqlTemplate
{
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class SearchProductsAsync : QueryAsyncBase<Product>
    {
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("select \r\n    ");
            this.Write(this.ToStringHelper.ToStringWithCulture(nameof(Product.ProductID)));
            this.Write(",\r\n    ");
            this.Write(this.ToStringHelper.ToStringWithCulture(nameof(Product.Name)));
            this.Write("\r\nfrom\r\n    Product\r\nwhere\r\n\t1 = 1\r\n");
 if(ProductID is not null) {
            this.Write("    and ProductID = @ProductId\r\n");
 } 
 if(Name is not null) {
            this.Write("\tand Name like @Name\r\n");
 } 
            return this.GenerationEnvironment.ToString();
        }
    }
}


select '[deORO_APlusVending]', barcode, count(*) from [deORO_APlusVending].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_Arca]', barcode, count(*) from [deORO_Arca].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_AutomaticSales]', barcode, count(*) from [deORO_AutomaticSales].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_CarsonCity]', barcode, count(*) from [deORO_CarsonCity].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_ConsolidatedVending]', barcode, count(*) from [deORO_ConsolidatedVending].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_DenverHealthy]', barcode, count(*) from [deORO_DenverHealthy].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_DoveVending]', barcode, count(*) from [deORO_DoveVending].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_Empire]', barcode, count(*) from [deORO_Empire].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_GoodStuffVending]', barcode, count(*) from [deORO_GoodStuffVending].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_JustRightVending]', barcode, count(*) from [deORO_JustRightVending].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_LakeViewVending]', barcode, count(*) from [deORO_LakeViewVending].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_Local_Backup]', barcode, count(*) from [deORO_Local_Backup].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_Mcliff]', barcode, count(*) from [deORO_Mcliff].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_OrlandoVending]', barcode, count(*) from [deORO_OrlandoVending].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_QualitySales]', barcode, count(*) from [deORO_QualitySales].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_SaltLakeVending]', barcode, count(*) from [deORO_SaltLakeVending].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_SantaClarita]', barcode, count(*) from [deORO_SantaClarita].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_SantaClaritaTest]', barcode, count(*) from [deORO_SantaClaritaTest].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_SchmidtVending]', barcode, count(*) from [deORO_SchmidtVending].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_Servco]', barcode, count(*) from [deORO_Servco].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_SitkaVending]', barcode, count(*) from [deORO_SitkaVending].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_SnackRevolution]', barcode, count(*) from [deORO_SnackRevolution].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_Sullivan]', barcode, count(*) from [deORO_Sullivan].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_Test]', barcode, count(*) from [deORO_Test].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_VendWatch]', barcode, count(*) from [deORO_VendWatch].dbo.item
group by barcode
having count(*) > 1
union all
select '[deORO_WestValley]', barcode, count(*) from [deORO_WestValley].dbo.item
group by barcode
having count(*) > 1;



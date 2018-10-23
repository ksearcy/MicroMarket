select '[deORO_APlusVending]', code, count(*) from [deORO_APlusVending].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_Arca]', code, count(*) from [deORO_Arca].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_AutomaticSales]', code, count(*) from [deORO_AutomaticSales].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_CarsonCity]', code, count(*) from [deORO_CarsonCity].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_ConsolidatedVending]', code, count(*) from [deORO_ConsolidatedVending].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_DenverHealthy]', code, count(*) from [deORO_DenverHealthy].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_DoveVending]', code, count(*) from [deORO_DoveVending].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_Empire]', code, count(*) from [deORO_Empire].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_GoodStuffVending]', code, count(*) from [deORO_GoodStuffVending].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_JustRightVending]', code, count(*) from [deORO_JustRightVending].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_LakeViewVending]', code, count(*) from [deORO_LakeViewVending].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_Local_Backup]', code, count(*) from [deORO_Local_Backup].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_Mcliff]', code, count(*) from [deORO_Mcliff].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_OrlandoVending]', code, count(*) from [deORO_OrlandoVending].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_QualitySales]', code, count(*) from [deORO_QualitySales].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_SaltLakeVending]', code, count(*) from [deORO_SaltLakeVending].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_SantaClarita]', code, count(*) from [deORO_SantaClarita].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_SantaClaritaTest]', code, count(*) from [deORO_SantaClaritaTest].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_SchmidtVending]', code, count(*) from [deORO_SchmidtVending].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_Servco]', code, count(*) from [deORO_Servco].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_SitkaVending]', code, count(*) from [deORO_SitkaVending].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_SnackRevolution]', code, count(*) from [deORO_SnackRevolution].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_Sullivan]', code, count(*) from [deORO_Sullivan].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_Test]', code, count(*) from [deORO_Test].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_VendWatch]', code, count(*) from [deORO_VendWatch].dbo.manufacturer
group by code
having count(*) > 1
union all
select '[deORO_WestValley]', code, count(*) from [deORO_WestValley].dbo.manufacturer
group by code
having count(*) > 1;




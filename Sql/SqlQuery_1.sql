select * from location

with s1 as 
(
	select shoppingcartpkid,sum(price_tax_included) as detail_amount from
	shoppingcartdetail where locationid = 11 AND created_date_time between '01/01/2016' and '01/05/2016'
	group by shoppingcartpkid
)
, s2 as 
(
	select shoppingcartpkid ,sum(amount) as payment_amount from
	payment where locationid = 11 AND created_date_time between '01/01/2016' and '01/05/2016'
	group by shoppingcartpkid
)
select s1.shoppingcartpkid,ceiling( (s1.detail_amount / 0.05 )) * 0.05 as rounded, s1.detail_amount,s2.payment_amount
from s1 inner join
s2
on s1.shoppingcartpkid = s2.shoppingcartpkid
WHERE ceiling( (s1.detail_amount / 0.05 )) * 0.05 <> s2.payment_amount
AND detail_amount <> payment_amount


select * from shoppingcartdetail where shoppingcartpkid = '92b33297-98da-4cc1-a0a8-faa319f608bb'
select * from payment where shoppingcartpkid = '92b33297-98da-4cc1-a0a8-faa319f608bb'


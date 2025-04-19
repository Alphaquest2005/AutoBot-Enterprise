select  round(sum(Net_weight),2) as weight from PreviousItemsEx where Prev_reg_nbr = '36816' and pLineNumber = 50
group by  registrationdate
order by RegistrationDate 

select  round(sum(Net_weight),2) as weight from PreviousItemsEx where Prev_reg_nbr = '36816' and pLineNumber = 50
group by  registrationdate, Net_weight
order by RegistrationDate 
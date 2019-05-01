UPDATE xcuda_Office_segment
SET       Customs_clearance_office_code = SUBSTRING(Customs_clearance_office_code, 1, 5)
WHERE (LEN(Customs_clearance_office_code) > 5)
1. Go to http://innosoft.ca/MagTekSetup/tabid/1971/Default.aspx
And follow the procedure listed there up to point 5
as listed below
-------------------------------------------------
1. After downloading the MagTek UPOS Service Object driver, 
http://www.magtek.com/support/software/programming_tools/99510087-100.exe
run the 99510087-100.exe file and follow the steps of the install wizard.
2. After the installation is complete use Windows Explorer and navigate to the
C:\Program Files\MagTek\MagTek UPOS SO
folder on your Windows System directory.
3. Copy the MagTekServiceObject.dll file to the
4. C:\Program Files\Common Files\Microsoft Shared\Point Of Service\Control Assemblies
folder on your Windows System directory.
5. Reboot the computer

-------------------------------------------------
6. Go to http://seanliming.com/POSforNET.html
7. From http://www.seanliming.com/Docs/WEPOS/SOMgr.zip download SOManager
8. Open it
9. Get POS Devices and select it
10. Find MagTek Msr object
in Logical name type: ePort_MagTek
and Add Logical name click
11. Reboot
---------------------------------------------------
to test
12. cmd
13. cd C:\Program Files (x86)\Microsoft Point Of Service
14. posdm listnames
15. if not listed, try adding by posdm
run cmd as admininstrator
cd C:\Program Files (x86)\Microsoft Point Of Service
posdm addname ePort_MagTek /TYPE:Msr /SONAME:"MagTek Msr"

to delete a name use
posdm deletename ePort_MagTek /TYPE:Msr /SONAME:"MagTek Msr"
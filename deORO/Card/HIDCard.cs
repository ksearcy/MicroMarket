using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Threading;
using System.Timers;
using Microsoft.Practices.Composite.Events;
using deORO.EventAggregation;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;



namespace deORO.ContactclessReader
{
    public class HIDCard
    {


        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        
        /**************************************************/
        //////////////////Global Variables//////////////////
        /**************************************************/
        IntPtr hContext;                                        //Context Handle value
        String readerName;                                      //Global Reader Variable
        int retval;                                             //Return Value
        uint dwscope;                                           //Scope of the resource manager context
        Boolean IsAuthenticated;                                //Boolean variable to check the authentication
        Boolean release_flag;                                   //Flag to release 
        IntPtr hCard;                                           //Card handle
        IntPtr protocol;                                        //Protocol used currently
        Byte[] ATR = new Byte[33];                              //Array stores Card ATR
        int card_Type;                                          //Stores the card type
        Byte[] sendBuffer = new Byte[255];                        //Send Buffer in SCardTransmit
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x16)]
        // public byte receiveBuffer;
        Byte[] receiveBuffer = new Byte[255];                   //Receive Buffer in SCardTransmit
        int sendbufferlen, receivebufferlen;                    //Send and Receive Buffer length in SCardTransmit
        Byte bcla;                                             //Class Byte
        Byte bins;                                             //Instruction Byte
        Byte bp1;                                              //Parameter Byte P1
        Byte bp2;                                              //Parameter Byte P2
        Byte len;                                              //Lc/Le Byte
        Byte[] data = new Byte[255];                            //Data Bytes
        HiDWinscard.SCARD_READERSTATE ReaderState;              //Object of SCARD_READERSTATE
        int value_Timeout;                                      //The maximum amount of time to wait for an action
        uint ReaderCount;                                       //Count for number of readers
        String ReaderList;                                      //List Of Reader
        System.Object sender1;                                  //Object of the Sender
        System.Windows.RoutedEventArgs e1;                      //Object of the Event
        Byte currentBlock;                                      //Stores the current block selected
        //String keych;                                           //Stores the string in key textbox
        int discarded;                                          //Stores the number of discarded character
        public delegate void DelegateTimer();                   //delegate of the Timer
        private System.Timers.Timer timer;                      //Object of the Timer
        public bool bTxtWrongInputChange;                       //Variable to check the wrong input in key textbox. Used in text change event
        bool read_pressed;                                      //flag to check read pressed
        string cardsSerialNumber;
        bool isReaderConnected = false;
        bool isCardInserted = false;
        //******************************************************************************************
        // Event Name : Handle
        // Description: Use to Handle Event on Click of About Button in System Menu 
        //******************************************************************************************
 


        public void Init()
        {
            //try
            //    {
            //        ReaderHelper.ConnectToReaderWithCard(reader);
            //        ReaderHelper.GetDataCommand(reader, "Get Data Command", GetDataCommand.Type.Default);
            //    }
            //    catch (Exception ex)
            //    {
            //        aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(new DeviceFailEventArgs()
            //        {
            //            DeviceType = "HIDCard",
            //            Message = ex.Message + " " + ex.StackTrace
            //        });
            //    }

            uint pcchReaders = 0;
            int nullindex = -1;
            char nullchar = (char)0;
            dwscope = 2;

            // Establish context.
            retval = HID.SCardEstablishContext(dwscope, IntPtr.Zero, IntPtr.Zero, out hContext);
            retval = HID.SCardListReaders(hContext, null, null, ref pcchReaders);
            byte[] mszReaders = new byte[pcchReaders];

            // Fill readers buffer with second call.
            retval = HID.SCardListReaders(hContext, null, mszReaders, ref pcchReaders);

            // Populate List with readers.
            string currbuff = Encoding.ASCII.GetString(mszReaders);
            ReaderList = currbuff;
            int len = (int)pcchReaders;

            if (len > 0)
            {
                while (currbuff[0] != nullchar)
                {
                    nullindex = currbuff.IndexOf(nullchar);   // Get null end character.
                    string reader = currbuff.Substring(0, nullindex);
                    //selectreadercombobox.Items.Add(reader);
                    readerName = reader;
                    len = len - (reader.Length + 1);
                    currbuff = currbuff.Substring(nullindex + 1, len);
                }
            }

            ////Tool Tip
            //Tool_Tip();
            ////KeyTextBox.IsEnabled = false;   //key textbox enabled false
            ////**************************************System Menu Add About Box Dialog***************

            //IntPtr systemMenuHandle = HID.GetSystemMenu(this.Handle, false);
            //HID.InsertMenu(systemMenuHandle, 7, HID.MF_BYPOSITION, HID._AboutSysMenuID, "About ...");

            //HwndSource source = HwndSource.FromHwnd(this.Handle);
            //source.AddHook(new HwndSourceHook(WndProc));

            ////*************************************************************************************
            //HiDLicense HD = new HiDLicense();
            //HD.Display_License();
            //string str;
            //str = HD.hidlicense;

            //============================Automatic Connection=========================================
            try
            {
                dwscope = 2;
                if (readerName != "" && readerName != null)
                {
                    retval = HID.SCardEstablishContext(dwscope, IntPtr.Zero, IntPtr.Zero, out hContext);
                    if (retval == 0)
                    {
                        //IsAuthenticated = false;
                        //sCardEstablishContextButton.IsEnabled = false;
                        isReaderConnected = true;
                        //ConnectButton.IsEnabled = true;
                        isCardInserted = false;
                        //Textcolorchange("> SCardEstablishContext" + "  Successful \n", System.Windows.Media.Brushes.Black);
                        //rtb.ScrollToEnd();
                        release_flag = true;
                    }
                    else
                    {
                        //Textcolorchange("> SCardEstablishContext" + "   Failed... " + "   Error Code: " + String.Format("{0:x}", retval) + "H\n", System.Windows.Media.Brushes.Red);
                        //rtb.ScrollToEnd();
                        timer.Enabled = false;
                    }
                }
                else
                {
                    //Textcolorchange("> SCardEstablishContext" + "   Failed... " + "   Error Code: " + String.Format("{0:x}", retval) + "H\n", System.Windows.Media.Brushes.Red);
                    //rtb.ScrollToEnd();
                    timer.Enabled = false;
                }

                //sender1 = sender;
                //e1 = e;

                ////////////////////Timer//////////////////////////////

                // Creating a timer with a ten second interval.
                timer = new System.Timers.Timer(1000);
                // Hook up the Elapsed event for the timer.
                timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                timer.Enabled = true;
            }
            catch { }

            //===========================================================================================
            
        }

        public void Dispose(){
        
                    retval = HID.SCardReleaseContext(hContext);
            if (retval == 0)
            {

                isReaderConnected = false;
                isCardInserted = false;
                timer.Enabled = false;
                //CardStatusTextBox.Text = "";
                //keynumberComboBox.Text = "";
                //LogTextBox.Text = "";
                //ConnectButton.IsEnabled = false;
                //sCardEstablishContextButton.IsEnabled = true;
                //SCardReleaseContextButton.IsEnabled = false;                
                //CardStatusTextBox.Background = System.Windows.Media.Brushes.White;
                //CardStatusTextBox.Foreground = System.Windows.Media.Brushes.Black;
                //Disabled_Enabled_Controls();
                //Textcolorchange("> SCardReleaseContext" + "   Successful \n", System.Windows.Media.Brushes.Black);
                //rtb.ScrollToEnd();              

            }
            else
            {
                //Textcolorchange("> SCardReleaseContext" + "   Failed... " + "   Error Code: " + String.Format("{0:x}", retval) + "H\n", System.Windows.Media.Brushes.Red);
                //rtb.ScrollToEnd();
                timer.Enabled = false;               

            }
        
        }

        //********************************************************
        //Function Name:OnTimedEvent
        //Input(Parameter) : source, e
        //OutPutParameter:-------
        //Description:Specify what you want to happen when the Elapsed event is raised.
        //********************************************************
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new DelegateTimer(timerWorkItem));
        }

        //********************************************************
        //Function Name: timerWorkItem
        //Input Parameter:-------
        //OutPutParameter:-------
        //Description:Perform action after time interval passed 
        //********************************************************
        private void timerWorkItem()
        {

            ReaderState.RdrName = readerName;
            ReaderState.RdrCurrState = HiDWinscard.SCARD_STATE_UNAWARE;
            ReaderState.RdrEventState = 0;
            ReaderState.UserData = "Mifare Card";
            value_Timeout = 0;
            ReaderCount = 1;

            if (isReaderConnected == true)
            {
                if (ReaderList == "")
                {
                    //CardStatusTextBox.Text = "SmartCard Removed";
                    //CardStatusTextBox.Background = System.Windows.Media.Brushes.Red;
                    //CardStatusTextBox.Foreground = System.Windows.Media.Brushes.White;
                }
                else
                {
                    retval = HID.SCardGetStatusChange(hContext, value_Timeout, ref ReaderState, ReaderCount);
                    if ((ReaderState.ATRLength == 0) || (retval != 0))
                    {
                        //CardStatusTextBox.Text = "SmartCard Removed";
                        //CardStatusTextBox.Background = System.Windows.Media.Brushes.Red;
                        //CardStatusTextBox.Foreground = System.Windows.Media.Brushes.White;
                        //DisconnectButton_Click(sender1, e1);
                        //mifarecardtypeLabel.Content = "";
                        //uidLabel.Content = "";
                    }
                    else
                    {
                        //CardStatusTextBox.Text = "SmartCard Inserted";
                        //CardStatusTextBox.Background = System.Windows.Media.Brushes.GreenYellow;
                        //CardStatusTextBox.Foreground = System.Windows.Media.Brushes.Black;

                        //========================Card Information===============================

                        {
                            retval = HID.SCardConnect(hContext, readerName, HiDWinscard.SCARD_SHARE_SHARED, HiDWinscard.SCARD_PROTOCOL_T1,
                                             ref hCard, ref protocol
                                              );       //Command to connect the card ,protocol T=1
                        }

                        ReaderState.RdrName = readerName;
                        ReaderState.RdrCurrState = HiDWinscard.SCARD_STATE_UNAWARE;
                        ReaderState.RdrEventState = 0;
                        ReaderState.UserData = "Mifare Card";
                        value_Timeout = 0;
                        ReaderCount = 1;

                        if (retval == 0)
                        {
                            //Textcolorchange("> SCardConnect" + "   Successful \n", System.Windows.Media.Brushes.Black);
                            //rtb.ScrollToEnd();
                            retval = HID.SCardGetStatusChange(hContext, value_Timeout, ref ReaderState, ReaderCount);

                            if (ReaderState.ATRValue[ReaderState.ATRLength - 0x6].Equals(1))
                            {
                                card_Type = 1;
                                ATR_UID(card_Type);
                            }
                            else if (ReaderState.ATRValue[ReaderState.ATRLength - 0x6].Equals(2))
                            {
                                card_Type = 2;
                                ATR_UID(card_Type);
                            }
                            else
                            {
                                card_Type = 3;
                                ATR_UID(card_Type);
                            }
                        }
                        else if (retval != 0 && isCardInserted == false)
                        {
                            //Textcolorchange("> SCardConnect" + "   Failed... " + "   Error Code: " + String.Format("{0:x}", retval) + "H\n", System.Windows.Media.Brushes.Red);
                            //rtb.ScrollToEnd();
                            timer.Enabled = true;
                        }

                        //========================Card Information===============================

                    }

                }
            }
        }


        //********************************************************
        //Function Name:card_Type_Identification
        //Description:Function to add blocks in block combo box based on card type
        //********************************************************
        //private void card_Type_Identification()
        //{
        //    BlockComboBox.Items.Clear();
        //    if (card_Type == HiDWinscard.card_Type_Mifare_1K)
        //    {
        //        BlockComboBox.Items.Add("00");
        //        BlockComboBox.Items.Add("01");
        //        BlockComboBox.Items.Add("02");
        //        BlockComboBox.Items.Add("03");
        //        BlockComboBox.Items.Add("04");
        //        BlockComboBox.Items.Add("05");
        //        BlockComboBox.Items.Add("06");
        //        BlockComboBox.Items.Add("07");
        //        BlockComboBox.Items.Add("08");
        //        BlockComboBox.Items.Add("09");
        //        BlockComboBox.Items.Add("10");
        //        BlockComboBox.Items.Add("11");
        //        BlockComboBox.Items.Add("12");
        //        BlockComboBox.Items.Add("13");
        //        BlockComboBox.Items.Add("14");
        //        BlockComboBox.Items.Add("15");
        //        BlockComboBox.Items.Add("16");
        //        BlockComboBox.Items.Add("17");
        //        BlockComboBox.Items.Add("18");
        //        BlockComboBox.Items.Add("19");
        //        BlockComboBox.Items.Add("20");
        //        BlockComboBox.Items.Add("21");
        //        BlockComboBox.Items.Add("22");
        //        BlockComboBox.Items.Add("23");
        //        BlockComboBox.Items.Add("24");
        //        BlockComboBox.Items.Add("25");
        //        BlockComboBox.Items.Add("26");
        //        BlockComboBox.Items.Add("27");
        //        BlockComboBox.Items.Add("28");
        //        BlockComboBox.Items.Add("29");
        //        BlockComboBox.Items.Add("30");
        //        BlockComboBox.Items.Add("31");
        //        BlockComboBox.Items.Add("32");
        //        BlockComboBox.Items.Add("33");
        //        BlockComboBox.Items.Add("34");
        //        BlockComboBox.Items.Add("35");
        //        BlockComboBox.Items.Add("36");
        //        BlockComboBox.Items.Add("37");
        //        BlockComboBox.Items.Add("38");
        //        BlockComboBox.Items.Add("39");
        //        BlockComboBox.Items.Add("40");
        //        BlockComboBox.Items.Add("41");
        //        BlockComboBox.Items.Add("42");
        //        BlockComboBox.Items.Add("43");
        //        BlockComboBox.Items.Add("44");
        //        BlockComboBox.Items.Add("45");
        //        BlockComboBox.Items.Add("46");
        //        BlockComboBox.Items.Add("47");
        //        BlockComboBox.Items.Add("48");
        //        BlockComboBox.Items.Add("49");
        //        BlockComboBox.Items.Add("50");
        //        BlockComboBox.Items.Add("51");
        //        BlockComboBox.Items.Add("52");
        //        BlockComboBox.Items.Add("53");
        //        BlockComboBox.Items.Add("54");
        //        BlockComboBox.Items.Add("55");
        //        BlockComboBox.Items.Add("56");
        //        BlockComboBox.Items.Add("57");
        //        BlockComboBox.Items.Add("58");
        //        BlockComboBox.Items.Add("59");
        //        BlockComboBox.Items.Add("60");
        //        BlockComboBox.Items.Add("61");
        //        BlockComboBox.Items.Add("62");
        //        BlockComboBox.Items.Add("63");
        //    }
        //    else if (card_Type == HiDWinscard.card_Type_Mifare_4K)
        //    {
        //        BlockComboBox.Items.Add("00");
        //        BlockComboBox.Items.Add("01");
        //        BlockComboBox.Items.Add("02");
        //        BlockComboBox.Items.Add("03");
        //        BlockComboBox.Items.Add("04");
        //        BlockComboBox.Items.Add("05");
        //        BlockComboBox.Items.Add("06");
        //        BlockComboBox.Items.Add("07");
        //        BlockComboBox.Items.Add("08");
        //        BlockComboBox.Items.Add("09");
        //        BlockComboBox.Items.Add("10");
        //        BlockComboBox.Items.Add("11");
        //        BlockComboBox.Items.Add("12");
        //        BlockComboBox.Items.Add("13");
        //        BlockComboBox.Items.Add("14");
        //        BlockComboBox.Items.Add("15");
        //        BlockComboBox.Items.Add("16");
        //        BlockComboBox.Items.Add("17");
        //        BlockComboBox.Items.Add("18");
        //        BlockComboBox.Items.Add("19");
        //        BlockComboBox.Items.Add("20");
        //        BlockComboBox.Items.Add("21");
        //        BlockComboBox.Items.Add("22");
        //        BlockComboBox.Items.Add("23");
        //        BlockComboBox.Items.Add("24");
        //        BlockComboBox.Items.Add("25");
        //        BlockComboBox.Items.Add("26");
        //        BlockComboBox.Items.Add("27");
        //        BlockComboBox.Items.Add("28");
        //        BlockComboBox.Items.Add("29");
        //        BlockComboBox.Items.Add("30");
        //        BlockComboBox.Items.Add("31");
        //        BlockComboBox.Items.Add("32");
        //        BlockComboBox.Items.Add("33");
        //        BlockComboBox.Items.Add("34");
        //        BlockComboBox.Items.Add("35");
        //        BlockComboBox.Items.Add("36");
        //        BlockComboBox.Items.Add("37");
        //        BlockComboBox.Items.Add("38");
        //        BlockComboBox.Items.Add("39");
        //        BlockComboBox.Items.Add("40");
        //        BlockComboBox.Items.Add("41");
        //        BlockComboBox.Items.Add("42");
        //        BlockComboBox.Items.Add("43");
        //        BlockComboBox.Items.Add("44");
        //        BlockComboBox.Items.Add("45");
        //        BlockComboBox.Items.Add("46");
        //        BlockComboBox.Items.Add("47");
        //        BlockComboBox.Items.Add("48");
        //        BlockComboBox.Items.Add("49");
        //        BlockComboBox.Items.Add("50");
        //        BlockComboBox.Items.Add("51");
        //        BlockComboBox.Items.Add("52");
        //        BlockComboBox.Items.Add("53");
        //        BlockComboBox.Items.Add("54");
        //        BlockComboBox.Items.Add("55");
        //        BlockComboBox.Items.Add("56");
        //        BlockComboBox.Items.Add("57");
        //        BlockComboBox.Items.Add("58");
        //        BlockComboBox.Items.Add("59");
        //        BlockComboBox.Items.Add("60");
        //        BlockComboBox.Items.Add("61");
        //        BlockComboBox.Items.Add("62");
        //        BlockComboBox.Items.Add("63");
        //        BlockComboBox.Items.Add("64");
        //        BlockComboBox.Items.Add("65");
        //        BlockComboBox.Items.Add("66");
        //        BlockComboBox.Items.Add("67");
        //        BlockComboBox.Items.Add("68");
        //        BlockComboBox.Items.Add("69");
        //        BlockComboBox.Items.Add("70");
        //        BlockComboBox.Items.Add("71");
        //        BlockComboBox.Items.Add("72");
        //        BlockComboBox.Items.Add("73");
        //        BlockComboBox.Items.Add("74");
        //        BlockComboBox.Items.Add("75");
        //        BlockComboBox.Items.Add("76");
        //        BlockComboBox.Items.Add("77");
        //        BlockComboBox.Items.Add("78");
        //        BlockComboBox.Items.Add("79");
        //        BlockComboBox.Items.Add("80");
        //        BlockComboBox.Items.Add("81");
        //        BlockComboBox.Items.Add("82");
        //        BlockComboBox.Items.Add("83");
        //        BlockComboBox.Items.Add("84");
        //        BlockComboBox.Items.Add("85");
        //        BlockComboBox.Items.Add("86");
        //        BlockComboBox.Items.Add("87");
        //        BlockComboBox.Items.Add("88");
        //        BlockComboBox.Items.Add("89");
        //        BlockComboBox.Items.Add("90");
        //        BlockComboBox.Items.Add("91");
        //        BlockComboBox.Items.Add("92");
        //        BlockComboBox.Items.Add("93");
        //        BlockComboBox.Items.Add("94");
        //        BlockComboBox.Items.Add("95");
        //        BlockComboBox.Items.Add("96");
        //        BlockComboBox.Items.Add("97");
        //        BlockComboBox.Items.Add("98");
        //        BlockComboBox.Items.Add("99");
        //        BlockComboBox.Items.Add("100");
        //        BlockComboBox.Items.Add("101");
        //        BlockComboBox.Items.Add("102");
        //        BlockComboBox.Items.Add("103");
        //        BlockComboBox.Items.Add("104");
        //        BlockComboBox.Items.Add("105");
        //        BlockComboBox.Items.Add("106");
        //        BlockComboBox.Items.Add("107");
        //        BlockComboBox.Items.Add("108");
        //        BlockComboBox.Items.Add("109");
        //        BlockComboBox.Items.Add("110");
        //        BlockComboBox.Items.Add("111");
        //        BlockComboBox.Items.Add("112");
        //        BlockComboBox.Items.Add("113");
        //        BlockComboBox.Items.Add("114");
        //        BlockComboBox.Items.Add("115");
        //        BlockComboBox.Items.Add("116");
        //        BlockComboBox.Items.Add("117");
        //        BlockComboBox.Items.Add("118");
        //        BlockComboBox.Items.Add("119");
        //        BlockComboBox.Items.Add("120");
        //        BlockComboBox.Items.Add("121");
        //        BlockComboBox.Items.Add("122");
        //        BlockComboBox.Items.Add("123");
        //        BlockComboBox.Items.Add("124");
        //        BlockComboBox.Items.Add("125");
        //        BlockComboBox.Items.Add("126");
        //        BlockComboBox.Items.Add("127");
        //        BlockComboBox.Items.Add("128");
        //        BlockComboBox.Items.Add("129");
        //        BlockComboBox.Items.Add("130");
        //        BlockComboBox.Items.Add("131");
        //        BlockComboBox.Items.Add("132");
        //        BlockComboBox.Items.Add("133");
        //        BlockComboBox.Items.Add("134");
        //        BlockComboBox.Items.Add("135");
        //        BlockComboBox.Items.Add("136");
        //        BlockComboBox.Items.Add("137");
        //        BlockComboBox.Items.Add("138");
        //        BlockComboBox.Items.Add("139");
        //        BlockComboBox.Items.Add("140");
        //        BlockComboBox.Items.Add("141");
        //        BlockComboBox.Items.Add("142");
        //        BlockComboBox.Items.Add("143");
        //        BlockComboBox.Items.Add("144");
        //        BlockComboBox.Items.Add("145");
        //        BlockComboBox.Items.Add("146");
        //        BlockComboBox.Items.Add("147");
        //        BlockComboBox.Items.Add("148");
        //        BlockComboBox.Items.Add("149");
        //        BlockComboBox.Items.Add("150");
        //        BlockComboBox.Items.Add("151");
        //        BlockComboBox.Items.Add("152");
        //        BlockComboBox.Items.Add("153");
        //        BlockComboBox.Items.Add("154");
        //        BlockComboBox.Items.Add("155");
        //        BlockComboBox.Items.Add("156");
        //        BlockComboBox.Items.Add("157");
        //        BlockComboBox.Items.Add("158");
        //        BlockComboBox.Items.Add("159");
        //        BlockComboBox.Items.Add("160");
        //        BlockComboBox.Items.Add("161");
        //        BlockComboBox.Items.Add("162");
        //        BlockComboBox.Items.Add("163");
        //        BlockComboBox.Items.Add("164");
        //        BlockComboBox.Items.Add("165");
        //        BlockComboBox.Items.Add("166");
        //        BlockComboBox.Items.Add("167");
        //        BlockComboBox.Items.Add("168");
        //        BlockComboBox.Items.Add("169");
        //        BlockComboBox.Items.Add("170");
        //        BlockComboBox.Items.Add("171");
        //        BlockComboBox.Items.Add("172");
        //        BlockComboBox.Items.Add("173");
        //        BlockComboBox.Items.Add("174");
        //        BlockComboBox.Items.Add("175");
        //        BlockComboBox.Items.Add("176");
        //        BlockComboBox.Items.Add("177");
        //        BlockComboBox.Items.Add("178");
        //        BlockComboBox.Items.Add("179");
        //        BlockComboBox.Items.Add("180");
        //        BlockComboBox.Items.Add("181");
        //        BlockComboBox.Items.Add("182");
        //        BlockComboBox.Items.Add("183");
        //        BlockComboBox.Items.Add("184");
        //        BlockComboBox.Items.Add("185");
        //        BlockComboBox.Items.Add("186");
        //        BlockComboBox.Items.Add("187");
        //        BlockComboBox.Items.Add("188");
        //        BlockComboBox.Items.Add("189");
        //        BlockComboBox.Items.Add("190");
        //        BlockComboBox.Items.Add("191");
        //        BlockComboBox.Items.Add("192");
        //        BlockComboBox.Items.Add("193");
        //        BlockComboBox.Items.Add("194");
        //        BlockComboBox.Items.Add("195");
        //        BlockComboBox.Items.Add("196");
        //        BlockComboBox.Items.Add("197");
        //        BlockComboBox.Items.Add("198");
        //        BlockComboBox.Items.Add("199");
        //        BlockComboBox.Items.Add("200");
        //        BlockComboBox.Items.Add("201");
        //        BlockComboBox.Items.Add("202");
        //        BlockComboBox.Items.Add("203");
        //        BlockComboBox.Items.Add("204");
        //        BlockComboBox.Items.Add("205");
        //        BlockComboBox.Items.Add("206");
        //        BlockComboBox.Items.Add("207");
        //        BlockComboBox.Items.Add("208");
        //        BlockComboBox.Items.Add("209");
        //        BlockComboBox.Items.Add("210");
        //        BlockComboBox.Items.Add("211");
        //        BlockComboBox.Items.Add("212");
        //        BlockComboBox.Items.Add("213");
        //        BlockComboBox.Items.Add("214");
        //        BlockComboBox.Items.Add("215");
        //        BlockComboBox.Items.Add("216");
        //        BlockComboBox.Items.Add("217");
        //        BlockComboBox.Items.Add("218");
        //        BlockComboBox.Items.Add("219");
        //        BlockComboBox.Items.Add("220");
        //        BlockComboBox.Items.Add("221");
        //        BlockComboBox.Items.Add("222");
        //        BlockComboBox.Items.Add("223");
        //        BlockComboBox.Items.Add("224");
        //        BlockComboBox.Items.Add("225");
        //        BlockComboBox.Items.Add("226");
        //        BlockComboBox.Items.Add("227");
        //        BlockComboBox.Items.Add("228");
        //        BlockComboBox.Items.Add("229");
        //        BlockComboBox.Items.Add("230");
        //        BlockComboBox.Items.Add("231");
        //        BlockComboBox.Items.Add("232");
        //        BlockComboBox.Items.Add("233");
        //        BlockComboBox.Items.Add("234");
        //        BlockComboBox.Items.Add("235");
        //        BlockComboBox.Items.Add("236");
        //        BlockComboBox.Items.Add("237");
        //        BlockComboBox.Items.Add("238");
        //        BlockComboBox.Items.Add("239");
        //        BlockComboBox.Items.Add("240");
        //        BlockComboBox.Items.Add("241");
        //        BlockComboBox.Items.Add("242");
        //        BlockComboBox.Items.Add("243");
        //        BlockComboBox.Items.Add("244");
        //        BlockComboBox.Items.Add("245");
        //        BlockComboBox.Items.Add("246");
        //        BlockComboBox.Items.Add("247");
        //        BlockComboBox.Items.Add("248");
        //        BlockComboBox.Items.Add("249");
        //        BlockComboBox.Items.Add("250");
        //        BlockComboBox.Items.Add("251");
        //        BlockComboBox.Items.Add("252");
        //        BlockComboBox.Items.Add("253");
        //        BlockComboBox.Items.Add("254");
        //        BlockComboBox.Items.Add("255");
        //    }
        //    else
        //    {
        //        ;
        //    }
        //}

        //********************************************************
        //Function Name:ATR_UID
        //Description:Gives ATR and UID of the card 
        //********************************************************

        private void ATR_UID(int card_type)
        {
            HiDWinscard.SCARD_IO_REQUEST sioreq;
            sioreq.dwProtocol = 0x2;
            sioreq.cbPciLength = 8;
            HiDWinscard.SCARD_IO_REQUEST rioreq;
            rioreq.cbPciLength = 8;
            rioreq.dwProtocol = 0x2;

            String uid_temp;
            String atr_temp;
            String s;
            atr_temp = "";
            uid_temp = "";
            s = "";
            StringBuilder hex = new StringBuilder(ReaderState.ATRValue.Length * 2);
            foreach (byte b in ReaderState.ATRValue)
                hex.AppendFormat("{0:X2}", b);
            atr_temp = hex.ToString();
            atr_temp = atr_temp.Substring(0, ((int)(ReaderState.ATRLength)) * 2);



            for (int k = 0; k <= ((ReaderState.ATRLength) * 2 - 1); k += 2)
            {
                s = s + atr_temp.Substring(k, 2) + " ";
            }

            atr_temp = s;

            bcla = 0xFF;
            bins = 0xCA;
            bp1 = 0x0;
            bp2 = 0x0;
            len = 0x0;
            sendBuffer[0] = bcla;
            sendBuffer[1] = bins;
            sendBuffer[2] = bp1;
            sendBuffer[3] = bp2;
            sendBuffer[4] = len;
            sendbufferlen = 0x5;
            receivebufferlen = 255;
            retval = HID.SCardTransmit(hCard, ref sioreq, sendBuffer, sendbufferlen, ref rioreq, receiveBuffer, ref receivebufferlen);
            if (retval == 0)
            {
                if ((receiveBuffer[receivebufferlen - 2] == 0x90) && (receiveBuffer[receivebufferlen - 1] == 0))
                {
                    StringBuilder hex1 = new StringBuilder((receivebufferlen - 2) * 2);
                    foreach (byte b in receiveBuffer)
                        hex1.AppendFormat("{0:X2}", b);
                    uid_temp = hex1.ToString();
                    uid_temp = uid_temp.Substring(0, ((int)(receivebufferlen - 2)) * 2);
                }
                else
                {
                    ;
                }
            }
            else
            {
                //Textcolorchange("> SCardTransmit" + "   Failed... " + "   Error Code: " + String.Format("{0:x}", retval) + "H\n", System.Windows.Media.Brushes.Red);
                //rtb.ScrollToEnd();
                timer.Enabled = false;
            }
            if (uid_temp == "")
            {
            }
            else
            {
                s = "";
                for (int k = 0; k <= ((receivebufferlen - 2) * 2 - 1); k += 2)
                {
                    s = s + uid_temp.Substring(k, 2) + " ";
                }
                uid_temp = s;
                cardsSerialNumber = uid_temp.Replace(" ","");



                aggregator.GetEvent<EventAggregation.HidCardLoginSuccessfulEvent>().Publish(cardsSerialNumber);

            }
            if (atr_temp.Length <= 66)
            {
                //atrLabel.Content = "ATR=" + atr_temp;
            }
            else
            {
                //atrLabel.Content = "ATR=" + atr_temp.Substring(1, 66) + "\n" + atr_temp.Substring(67, atr_temp.Length);
            }

                if (card_type == 1 || card_type == 2)
                {
                //    keyRadioButton.IsEnabled = true;
                //    keRadioButton.IsEnabled = true;
                //    {
                //        {
                //            keynumberComboBox.IsEnabled = true;
                //            keynumberComboBox.Text = "00";

                //        }
                //    }

                //    ConnectButton.IsEnabled = false;
                      isCardInserted = true;
                //    BlockComboBox.IsEnabled = true;
                //    AuthenticateButton.IsEnabled = true;

                //    keyloadTextBox.IsEnabled = true;
                //    keynumberloadComboBox.IsEnabled = true;
                //    keynumberloadComboBox.Text = "00";
                //    BlockComboBox.Text = "00";
                //    loadkeyButton.IsEnabled = true;
                //    CardStatusTextBox.Text = "SmartCard Inserted";
                //    CardStatusTextBox.Background = System.Windows.Media.Brushes.GreenYellow;
                //    CardStatusTextBox.Foreground = System.Windows.Media.Brushes.Black;
                //    card_Type_Identification();
                //    BlockComboBox.SelectedIndex = 0;

                }
                else
                {
                      isCardInserted = true;
                    //ConnectButton.IsEnabled = false;
                }

            //if (card_type == 1)
            //{
            //    mifarecardtypeLabel.Content = "Card Type: Mifare 1K";
            //}
            //else if (card_type == 2)
            //{
            //    mifarecardtypeLabel.Content = "Card Type: Mifare 4K";
            //}
            //else if (card_type == 3 && uid_temp == "")
            //{
            //    mifarecardtypeLabel.Content = "Card Type: No Mifare 1K or 4K Card ";
            //    cardsSerialNumber = "n/a";
            //}
            //else if (card_type == 3)
            //{
            //    mifarecardtypeLabel.Content = "Card Type: No Mifare 1K or 4K Card ";
            //}

        }

                //finally
                //{
                //    if (reader.IsConnected)
                //    {
                //        reader.Disconnect(CardDisposition.Unpower);
                //    }
                //}

            //ReaderCollection readerCollection = ReaderCollection.GetReaders();

        //    if (readerCollection.Count > 0)
        //    {
        //        try
        //        {
        //            reader = readerCollection[0];
        //            identification = new DPCtlUruNet.IdentificationControl(reader, fingerPrints,
        //                             Helpers.Global.FingerprintThresholdScore, 10, Constants.CapturePriority.DP_PRIORITY_EXCLUSIVE);
        //            identification.Name = "dpIdentificationCtrl";
        //            identification.OnIdentify += identification_OnIdentify;
        //            identification.StartIdentification();
                    
        //        }
        //        catch (Exception ex)
        //        {
        //            aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(new DeviceFailEventArgs()
        //            {
        //                DeviceType = Helpers.Enum.DeviceType.FingerPrintReader,
        //                Message = ex.Message + " " + ex.StackTrace
        //            });
        //        }
        //    }
        //}

        //public void LoadHIDCards()
        //{
        //    object[] o = membership.GetAllUsersForFmdIdentification();
        //    userNames = (List<string>)o[1];
        //    fingerPrints = (List<Fmd>)o[3];

        //}

        //void identification_OnIdentify(DPCtlUruNet.IdentificationControl IdentificationControl, IdentifyResult IdentificationResult)
        //{
        //    List<string> unFound = new List<string>();

        //    try
        //    {
        //        if (Helpers.Global.DemoMode)
        //        {
        //            aggregator.GetEvent<EventAggregation.FingerPrintLoginSuccessfulEvent>().Publish(userNames[0]);
        //            return;
        //        }
        //    }
        //    catch { }


        //    if (IdentificationResult.Indexes.Length <= 0)
        //    {
        //        //aggregator.GetEvent<EventAggregation.LoginFailEvent>().Publish(deORO.Helpers.Enum.AuthenticationMode.FingerPrint);
        //        aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new EventAggregation.TransactionErrorEventArgs
        //        {
        //            DeviceType = Helpers.Enum.DeviceType.Login,
        //            Event = "Login Failed. Invalid Fingerprint",
        //            ShoppingCartPkid = "FingerPrint",
        //            Code = "ZLN"
        //        });
        //    }
        //    else
        //    {
        //        for (int i = 0; i < IdentificationResult.Indexes.Length; i++)
        //        {
        //            if (!unFound.Contains(userNames[IdentificationResult.Indexes[i][0]])) // FMD index = 0
        //            {
        //                unFound.Add(userNames[IdentificationResult.Indexes[i][0]]);
        //            }
        //        }
        //    }

        //    if (unFound.Count >= 1)
        //    {
        //        aggregator.GetEvent<EventAggregation.FingerPrintLoginSuccessfulEvent>().Publish(unFound[0]);
        //    }
        //}

        //public void Dispose()
        //{
        //    if (identification != null)
        //    {
        //        reader.Reset();
        //        identification.StopIdentification();
        //        identification.OnIdentify -= identification_OnIdentify;
        //        identification = null;
        //    }
        //}


    }


    class HID
    {
        //*********************************************************************************************************
        // Define Constants, To Add "About Dialog Box" in System Menu
        //*********************************************************************************************************
        public const Int32 WM_SYSCOMMAND = 0x112;
        public const Int32 MF_SEPARATOR = 0x800;
        public const Int32 MF_BYPOSITION = 0x400;
        public const Int32 MF_STRING = 0x0;

        public const Int32 _SettingsSysMenuID = 1000;
        public const Int32 _AboutSysMenuID = 1001;


        //*********************************************************************************************************
        // Function Name: GetSystemMenu
        // In Parameter : hWnd - A handle to the window that will own a copy of the window menu.
        //                bRevert - The action to be taken. If this parameter is FALSE, GetSystemMenu returns a handle to the copy of the window menu currently in use. 
        // Out Parameter: ------
        // Description  : Enables the application to access the window menu for copying and modifying.
        //*********************************************************************************************************
        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);


        //*********************************************************************************************************
        // Function Name: InsertMenu
        // In Parameter : hWnd - A handle to the menu to be changed. 
        //                Position - The menu item before which the new menu item is to be inserted, as determined by the uFlags parameter. 
        //                Flag - Controls the interpretation of the uPosition parameter and the content, appearance, and behavior of the new menu item.
        //                IDNewItem - The identifier of the new menu item or, if the uFlags parameter has the MF_POPUP flag set, a handle to the drop-down menu or submenu.
        //                newItem - The content of the new menu item.
        // Out Parameter: ---------
        // Description  : Inserts a new menu item into a menu, moving other items down the menu.
        //*********************************************************************************************************
        [DllImport("user32.dll")]
        public static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem, string lpNewItem);


        // *********************************************************************************************************
        // Function Name: SCardEstablishContext
        // In Parameter : dwScope - Scope of the resource manager context.
        //                pvReserved1 - Reserved for future use and must be NULL
        //                pvReserved2 - Reserved for future use and must be NULL.
        // Out Parameter: phContext - A handle to the established resource manager context
        // Description  : Establishes context to the reader
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardEstablishContext(uint dwScope,
        IntPtr notUsed1,
        IntPtr notUsed2,
        out IntPtr phContext);


        // *********************************************************************************************************
        // Function Name: SCardReleaseContext
        // In Parameter : phContext - A handle to the established resource manager context              
        // Out Parameter: -------
        // Description  :Releases context from the reader
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardReleaseContext(IntPtr phContext);


        // *********************************************************************************************************
        // Function Name: SCardConnect
        // In Parameter : hContext - A handle that identifies the resource manager context.
        //                cReaderName  - The name of the reader that contains the target card.
        //                dwShareMode - A flag that indicates whether other applications may form connections to the card.
        //                dwPrefProtocol - A bitmask of acceptable protocols for the connection.  
        // Out Parameter: ActiveProtocol - A flag that indicates the established active protocol.
        //                hCard - A handle that identifies the connection to the smart card in the designated reader. 
        // Description  : Connect to card on reader
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardConnect(IntPtr hContext,
        string cReaderName,
        uint dwShareMode,
        uint dwPrefProtocol,
        ref IntPtr hCard,
        ref IntPtr ActiveProtocol);


        // *********************************************************************************************************
        // Function Name: SCardDisconnect
        // In Parameter : hCard - Reference value obtained from a previous call to SCardConnect.
        //                Disposition - Action to take on the card in the connected reader on close.  
        // Out(Parameter)
        // Description  : Disconnect card from reader
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardDisconnect(IntPtr hCard, int Disposition);


        //    *********************************************************************************************************
        // Function Name: SCardListReaders
        // In Parameter : hContext - A handle to the established resource manager context
        //                mszReaders - Multi-string that lists the card readers with in the supplied readers groups
        //                pcchReaders - length of the readerlist buffer in characters
        // Out Parameter: mzGroup - Names of the Reader groups defined to the System
        //                pcchReaders - length of the readerlist buffer in characters
        // Description  : List of all readers connected to system 
        //*********************************************************************************************************
        [DllImport("WinScard.dll", EntryPoint = "SCardListReadersA", CharSet = CharSet.Ansi)]
        public static extern int SCardListReaders(
          IntPtr hContext,
          byte[] mszGroups,
          byte[] mszReaders,
          ref UInt32 pcchReaders
          );


        // *********************************************************************************************************
        // Function Name: SCardState
        // In Parameter : hCard - Reference value obtained from a previous call to SCardConnect.
        // Out Parameter: state - Current state of smart card in  the reader
        //                protocol - Current Protocol
        //                ATR - 32 bytes buffer that receives the ATR string
        //                ATRLen - Supplies the length of ATR buffer
        // Description  : Current state of the smart card in the reader
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardState(IntPtr hCard, ref IntPtr state, ref IntPtr protocol, ref Byte[] ATR, ref int ATRLen);


        // *********************************************************************************************************
        // Function Name: SCardTransmit
        // In Parameter : hCard - A reference value returned from the SCardConnect function.
        //                pioSendRequest - A pointer to the protocol header structure for the instruction.
        //                SendBuff- A pointer to the actual data to be written to the card.
        //                SendBuffLen - The length, in bytes, of the pbSendBuffer parameter. 
        //                pioRecvRequest - Pointer to the protocol header structure for the instruction ,Pointer to the protocol header structure for the instruction, 
        //                followed by a buffer in which to receive any returned protocol control information (PCI) specific to the protocol in use.
        //                RecvBuffLen - Supplies the length, in bytes, of the pbRecvBuffer parameter and receives the actual number of bytes received from the smart card.
        // Out Parameter: pioRecvRequest - Pointer to the protocol header structure for the instruction ,Pointer to the protocol header structure for the instruction, 
        //                followed by a buffer in which to receive any returned protocol control information (PCI) specific to the protocol in use.
        //                RecvBuff - Pointer to any data returned from the card.
        //                RecvBuffLen - Supplies the length, in bytes, of the pbRecvBuffer parameter and receives the actual number of bytes received from the smart card.
        // Description  : Transmit APDU to card 
        //*********************************************************************************************************
        [DllImport("WinScard.dll")]
        public static extern int SCardTransmit(IntPtr hCard, ref HiDWinscard.SCARD_IO_REQUEST pioSendRequest,
                                                           Byte[] SendBuff,
                                                           int SendBuffLen,
                                                           ref HiDWinscard.SCARD_IO_REQUEST pioRecvRequest,
                                                           Byte[] RecvBuff, ref int RecvBuffLen);


        // *********************************************************************************************************
        // Function Name: SCardGetStatusChange
        // In Parameter : hContext - A handle that identifies the resource manager context.
        //                value_TimeOut - The maximum amount of time, in milliseconds, to wait for an action.
        //                ReaderState -  An array of SCARD_READERSTATE structures that specify the readers to watch, and that receives the result.
        //                ReaderCount -  The number of elements in the rgReaderStates array.
        // Out Parameter: ReaderState - An array of SCARD_READERSTATE structures that specify the readers to watch, and that receives the result.
        // Description  : The current availability of the cards in a specific set of readers changes.
        //*********************************************************************************************************
        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        public static extern int SCardGetStatusChange(IntPtr hContext,
        int value_Timeout,
        ref HiDWinscard.SCARD_READERSTATE ReaderState,
        uint ReaderCount);

    }

    //Class for Constants
    public class HiDWinscard
    {
        // Context Scope

        public const int SCARD_STATE_UNAWARE = 0x0;

        //The application is unaware about the curent state, This value results in an immediate return
        //from state transition monitoring services. This is represented by all bits set to zero

        public const int SCARD_SHARE_SHARED = 2;

        // Application will share this card with other 
        // applications.

        //   Disposition

        public const int SCARD_UNPOWER_CARD = 2; // Power down the card on close

        //   PROTOCOL

        public const int SCARD_PROTOCOL_T0 = 0x1;                  // T=0 is the active protocol.
        public const int SCARD_PROTOCOL_T1 = 0x2;                  // T=1 is the active protocol.
        public const int SCARD_PROTOCOL_UNDEFINED = 0x0;

        //IO Request Control
        public struct SCARD_IO_REQUEST
        {
            public int dwProtocol;
            public int cbPciLength;
        }


        //Reader State

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SCARD_READERSTATE
        {
            public string RdrName;
            public string UserData;
            public uint RdrCurrState;
            public uint RdrEventState;
            public uint ATRLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x24, ArraySubType = UnmanagedType.U1)]
            public byte[] ATRValue;
        }
        //Card Type
        public const int card_Type_Mifare_1K = 1;
        public const int card_Type_Mifare_4K = 2;

    }

    //**************************************************************************
    //class for Hexidecimal to Byte and Byte to Hexidecimal conversion
    //**************************************************************************
    public class HexToBytenByteToHex
    {
        public HexToBytenByteToHex()
        {

            // constructor

        }
        public static int GetByteCount(string hexString)
        {
            int numHexChars = 0;
            char c;
            // remove all none A-F, 0-9, characters
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (IsHexDigit(c))
                    numHexChars++;
            }
            // if odd number of characters, discard last character
            if (numHexChars % 2 != 0)
            {
                numHexChars--;
            }
            return numHexChars / 2; // 2 characters per byte
        }

        public static byte[] GetBytes(string hexString, out int discarded)
        {
            discarded = 0;
            string newString = "";
            char c;
            // remove all none A-F, 0-9, characters
            for (int i = 0; i < hexString.Length; i++)
            {
                c = hexString[i];
                if (IsHexDigit(c))
                    newString += c;
                else
                    discarded++;
            }
            // if odd number of characters, discard last character
            if (newString.Length % 2 != 0)
            {
                discarded++;
                newString = newString.Substring(0, newString.Length - 1);
            }

            int byteLength = newString.Length / 2;
            byte[] bytes = new byte[byteLength];
            string hex;
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                hex = new String(new Char[] { newString[j], newString[j + 1] });
                bytes[i] = HexToByte(hex);
                j = j + 2;
            }
            return bytes;
        }
        public static string ToString(byte[] bytes)
        {
            string hexString = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                hexString += bytes[i].ToString("X2");
            }
            return hexString;
        }
        public static bool InHexFormat(string hexString)
        {
            bool hexFormat = true;

            foreach (char digit in hexString)
            {
                if (!IsHexDigit(digit))
                {
                    hexFormat = false;
                    break;
                }
            }
            return hexFormat;
        }

        public static bool IsHexDigit(Char c)
        {
            int numChar;
            int numA = Convert.ToInt32('A');
            int num1 = Convert.ToInt32('0');
            c = Char.ToUpper(c);
            numChar = Convert.ToInt32(c);
            if (numChar >= numA && numChar < (numA + 6))
                return true;
            if (numChar >= num1 && numChar < (num1 + 10))
                return true;
            return false;
        }
        private static byte HexToByte(string hex)
        {
            if (hex.Length > 2 || hex.Length <= 0)
                throw new ArgumentException("hex must be 1 or 2 characters in length");
            byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return newByte;
        }


    }

}

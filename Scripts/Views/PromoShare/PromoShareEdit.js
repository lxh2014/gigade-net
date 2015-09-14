editFunction = function (row, store) {
    Ext.apply(Ext.form.VTypes, {
        dateRange: function (val, field) {
            if (field.dateRange) {
                var beginId = field.dateRange.begin;
                this.beginField = Ext.getCmp(beginId);
                var endId = field.dateRange.end;
                this.endField = Ext.getCmp(endId);
                var beginDate = this.beginField.getValue();
                var endDate = this.endField.getValue();
            }
            if (beginDate != null && endDate != null) {
                if (beginDate <= endDate) {
                    return true;
                } else {
                    return false;
                }
            }
            else {
                return true;
            }
        },
        //验证失败信息  
        dateRangeText: '开始日期不能大于结束日期'
    });
    var currentPanel = 0;
    //會員群組store
    var PromoId = 0;
    var navigate = function (panel, direction) {
        var layout = panel.getLayout();
        if ('next' == direction) {
            if (currentPanel == 0) {//首頁時進行第一步保存
                //Ext.getCmp('move-prev').setDisabled(false); //當前為第一個面板時隱藏prev
                var ffrom = firstForm.getForm();
                if (row == null) {
                    if (PromoId == 0) {//表示為新增
                        if (ffrom.isValid()) {
                            if (Ext.Date.format(new Date(Ext.getCmp('promo_start').getValue()), 'Y-m-d H:i:s') >Ext.Date.format(new Date(Ext.getCmp('promo_end').getValue()), 'Y-m-d H:i:s'))
                            {
                                //alert("error");
                                Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                                return;
                            }
                            Ext.Ajax.request({
                                url: '/PromoShare/InsertIntoPromoShareMaster',
                                params: {
                                    promo_name:Ext.htmlEncode( Ext.getCmp('promo_name').getValue()),
                                    promo_desc: Ext.htmlEncode(Ext.getCmp('promo_desc').getValue()),
                                    promo_start: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('promo_start').getValue()), 'Y-m-d H:i:s')),
                                    promo_end:Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('promo_end').getValue()), 'Y-m-d H:i:s')),
                                },
                                success: function (form, action) {
                                  
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        Ext.getCmp("promo_id").setValue(result.PromoId);
                                        Ext.getCmp("promo_event_id").setValue(result.strPromoId);
                                        PromoId = Ext.getCmp("promo_id").getValue();
                                        layout[direction]();
                                        currentPanel++;
                                        if (!layout.getNext()) {
                                            Ext.getCmp('move-next').hide();
                                            Ext.getCmp('move-prev').setDisabled(false); //當前為第一個面板時隱藏prev
                                        }
                                        else {
                                            Ext.getCmp('move-next').setText(NEXT_MOVE);
                                        }
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, FAILURE);
                                    }
                                },
                                failure: function () {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            });
                        }
                    }
                    else {
                        if (ffrom.isValid()) {

                            //Ext.Ajax.request({
                            //    url: "/PromoShare/EditPromoShareMaster",
                            //    params: {
                            //PromoId: PromoId,
                            //Promo_name: Ext.getCmp('promo_name').getValue(),
                            //Promo_desc: Ext.getCmp('promo_desc').getValue(),
                            //Promo_start: Ext.getCmp('promo_start').getValue(),
                            //Promo_end: Ext.getCmp('promo_end').getValue(),
                            //},
                            //    success: function () {

                            //    }
                            //});

                            layout[direction]();
                            currentPanel++;
                            if (!layout.getNext()) {
                                Ext.getCmp('move-next').hide();
                                Ext.getCmp('move-prev').setDisabled(false); //當前為第一個面板時隱藏prev
                            }
                            else {
                                Ext.getCmp('move-next').setText(NEXT_MOVE);
                            }
                        }
                    }
                }
                else {
                    layout[direction]();
                    currentPanel++;
                    if (!layout.getNext()) {
                        Ext.getCmp('move-next').hide();
                        Ext.getCmp('move-prev').setDisabled(false); //當前為第一個面板時隱藏prev
                    }
                    else {
                        Ext.getCmp('move-next').setText(NEXT_MOVE);
                    }
                }

            }
        }
        else {
            layout[direction]();
            currentPanel--;
            if (currentPanel == 0) {
                Ext.getCmp('move-next').show();
                Ext.getCmp('move-prev').setDisabled(true); //第一次新增時上一步隱藏
            }
        }
    };
    var firstForm = Ext.widget('form', {
        id: 'editFrm1',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        defaults: { anchor: "95%", labelWidth: 80, msgTarget: "side" },
        items: [

              {
                  fieldLabel: "活動名稱",
                  xtype: 'textfield',
                  margin: "5 5 0 0",
                  id: 'promo_name',
                  name: 'promo_name',
                  allowBlank: false
              },
            {
                fieldLabel: "活動描述",
                xtype: 'textfield',
                margin: "5 5 0 0",
                id: 'promo_desc',
                name: 'promo_desc',
                allowBlank: false
            },
                   {
                       xtype: 'datetimefield',
                       allowBlank: false,
                       id: 'promo_start',
                       margin: "5 5 0 0",
                       format: 'Y-m-d H:i:s',
                       name: 'promo_start',
                       fieldLabel: '開始時間',
                       dateRange: { begin: 'promo_start', end: 'promo_end' },
                       value: Tomorrow(),
                       vtype: 'dateRange'

                   },
            {
                xtype: 'datetimefield',
                allowBlank: false,
                id: 'promo_end',
                margin: "5 5 0 0",
                format: 'Y-m-d H:i:s',
                name: 'promo_end',
                fieldLabel: '結束時間',
                dateRange: { begin: 'promo_start', end: 'promo_end' },
                value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() + 1)),
                vtype: 'dateRange'

            }
        ]
    });
    var secondForm = Ext.widget('form', {
        id: 'editFrm2',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        url: '/PromoShare/updatePromoShareCon',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
             {
                 fieldLabel: "活動編號",
                 xtype: 'displayfield',
                 id: 'promo_id',
                 name: 'promo_id',
                 hidden:true,
             },
             {
                 fieldLabel: "活動編號",
                 xtype: 'displayfield',
                 id: 'promo_event_id',
                 name: 'promo_event_id',
             },
             {
                 xtype: 'combobox',
                 id: 'devicename',
                 name: 'devicename',
                 fieldLabel: "裝置",
                 store: deviceStore,
                 displayField: 'txt',
                 valueField:'value'
             },
             {
                 xtype: "numberfield",
                 fieldLabel: "商品編號",
                 id: 'product_id',
                 name: 'product_id',
                 allowBlank: true,
                 allowDecimals :false,
                 minValue: 0,
                 listeners: {
                     'change': function () {
                         var pid = Ext.getCmp("product_id");
                         if (pid.getValue() != "" && pid.getValue() != null) {
                             Ext.Ajax.request({
                                 url: '/PromotionsDiscount/GetProdName',
                                 params: {
                                     product_id: Ext.htmlEncode(pid.getValue()),
                                 },
                                 success: function (form, action) {
                                     var result = Ext.decode(form.responseText);
                                     if (result.success) {
                                         Ext.getCmp('proName').setValue(result.prod_name);
                                     } else {
                                         Ext.getCmp('proName').setValue(PRODTIP);
                                         //Ext.Msg.alert(INFORMATION, PRODTIP);
                                       //  pid.setValue(0);
                                     }
                                 },
                                 failure: function () {
                                     Ext.getCmp('proName').setValue(PRODTIP);
                                     //Ext.Msg.alert(INFORMATION, PRODTIP);
                                  //   pid.setValue(0);
                                 }
                             });
                         }
                     }

                 }
             },
             {
                 xtype: 'displayfield',
                 id: 'proName',
                 name: 'proName',
                 fieldLabel:'商品名稱'
             },
             {
                 xtype: 'fieldcontainer',
                 layout:'hbox',
                 items: [
                     {
                         xtype: 'numberfield',
                         id: 'after_user_time',
                         minValue: 0,
                         fieldLabel: '會員註冊時間距離'
                     },
                     {
                         xtype: 'displayfield',
                         value: '個月'
                     },
                 ]
             },
                          {
                              xtype: 'fieldcontainer',
                              layout: 'hbox',
                              items: [
                                  {
                                      xtype: 'numberfield',
                                      id: 'after_first_buy_time',
                                      minValue: 0,
                                      fieldLabel: '會員首購時間距離'
                                  },
                                  {
                                      xtype: 'displayfield',
                                      value: '個月'
                                  },
                              ]
                          },
                              {
                                  xtype: 'combobox',
                                  fieldLabel: '會員群組',
                                  store: VipGroupStore,
                                  id: 'group_id',
                                  displayField: 'group_name',
                                  valueField: 'group_id',
                                  editable: false,
                                  multiSelect: true,
                                  lastQuery: '',
                                  emptyText: '',
                                  //     queryMode: 'local',

                              },
            {
                xtype: 'combobox',
                fieldLabel: '付款方式',
                store: paymentStore,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                id: 'payType',
                editable: false,
                lastQuery: '',
                emptyText: '',
                //   queryMode: 'local',
                multiSelect: true
            },
              {
                  xtype: 'combobox',
                  fieldLabel: 'website設定',
                  id: 'site_id',
                  store: SiteStore,
                  displayField: 'Site_Name',
                  valueField: 'Site_Id',
                  editable: false,
                  lastQuery: '',
                  emptyText: '',
                  //   queryMode: 'local',
                  multiSelect: true
              },
             {
                 xtype: 'numberfield',
                 fieldLabel: '會員消費金額',
                 id: 'consume_money',
                 name: 'consume_money',
                 allowDecimals: false,
                 minValue: 0,
             },
               {
                   xtype: 'numberfield',
                   fieldLabel: '會員購買次數',
                   id: 'buy_count',
                   name: 'buy_count',
                   allowDecimals: false,
                   minValue: 0,
               },
             {
                 xtype: 'numberfield',
                 fieldLabel: '廠商回饋(%)',
                 id:'vendor_tip',
                 allowDecimals: false,
                 minValue:0,
             },
             {
                 xtype: 'numberfield',
                 fieldLabel: '購物金',
                 id: 'bonus',
                 name:'bonus',
                 allowDecimals: false,
                 minValue:0,
             },
                 {
                     xtype: 'numberfield',
                     fieldLabel: '抵用券',
                     id: 'voucher',
                     name: 'voucher',
                     allowDecimals: false,
                     minValue: 0,
                 },
                      {
                          xtype: 'numberfield',
                          fieldLabel: '運費金額',
                          id: 'freight',
                          name: 'freight',
                          allowDecimals: false,
                          minValue: 0,
                      },
                  {
                      xtype: 'combobox',
                      fieldLabel: '是否重複送',
                      store: repeatStore,
                      displayField: 'txt',
                      valueField:'value',
                      id: 'isrepeat',
                      name: 'isrepeat',
                      allowDecimals: false,
                      minValue: 0,
                  },
                   {
                       xtype: 'numberfield',
                       fieldLabel: '贈送倍數',
                       id: 'multiple',
                       name: 'multiple',
                       allowDecimals: false,
                       minValue: 0,
                   },
             {
                 xtype: 'textfield',
                 fieldLabel: '自訂url',
                 id: 'by_url',
                 name: 'by_url',
                 vtype:'url',
             },
             {
                 xtype: 'filefield',
                 fieldLabel:'圖片',
                 id: 'picture',
                 name: 'picture',
                 buttonText: '選擇圖片...'
             },
        
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    if (Ext.getCmp("proName").getValue() == PRODTIP)
                    {
                        Ext.Msg.alert(INFORMATION, PRODTIP);
                        return;
                    }
                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: 'Loading...' });
                    myMask.show();
                    form.submit(
                        {
                            params: {
                          promo_name: Ext.htmlEncode( Ext.getCmp('promo_name').getValue()),//活動名稱
                          promo_desc: Ext.htmlEncode(Ext.getCmp('promo_desc').getValue()),//活動描述
                          promo_start: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('promo_start').getValue()), 'Y-m-d H:i:s')),//開始時間
                          promo_end: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('promo_end').getValue()), 'Y-m-d H:i:s')),//結束時間
                          promo_id: Ext.htmlEncode(Ext.getCmp('promo_id').getValue()),//活動編號
                          promo_event_id: Ext.htmlEncode(Ext.getCmp('promo_event_id').getValue()),//活動編號
                          device: Ext.htmlEncode(Ext.getCmp('devicename').getValue()),//裝置
                          product_id: Ext.htmlEncode(Ext.getCmp('product_id').getValue()),//商品編號
                          after_user_time: Ext.htmlEncode(Ext.getCmp('after_user_time').getValue()),//會員註冊時間距離 個月
                          after_first_buy_time: Ext.htmlEncode(Ext.getCmp('after_first_buy_time').getValue()),//會員首次購買時間距離 個月
                          consume_money: Ext.htmlEncode(Ext.getCmp('consume_money').getValue()),//會員消費金額
                          buy_count: Ext.htmlEncode(Ext.getCmp('buy_count').getValue()),//會員購買次數
                          vendor_tip: Ext.htmlEncode(Ext.getCmp('vendor_tip').getValue()),//廠商回饋
                          bonus: Ext.htmlEncode(Ext.getCmp('bonus').getValue()),//購物金
                          voucher: Ext.htmlEncode(Ext.getCmp('voucher').getValue()),//抵用券
                          freight: Ext.htmlEncode(Ext.getCmp('freight').getValue()),//運費金額
                          isrepeat: Ext.htmlEncode(Ext.getCmp('isrepeat').getValue()),//是否重覆送
                          multiple: Ext.htmlEncode(Ext.getCmp('multiple').getValue()),//贈送倍數
                          by_url: Ext.htmlEncode(Ext.getCmp('by_url').getValue()),//自訂URL
                          picture: Ext.htmlEncode(Ext.getCmp('picture').getValue()),//圖片
                          group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),//會員羣組
                          payType: Ext.htmlEncode(Ext.getCmp('payType').getValue()),//付款方式
                          site_id: Ext.htmlEncode(Ext.getCmp('site_id').getValue())//website設定
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                myMask.hide();
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                editWin.close();
                                store.load();
                            }
                            else {
                                myMask.hide();
                                Ext.Msg.alert(INFORMATION, FAILURE);
                                editWin.close();
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                            editWin.close();
                        }
                    }
                    )
                }
            }
        }]
    });

    var allpan = new Ext.panel.Panel({
        width: 390,
        layout: 'card',
        border: 0,
        bodyStyle: 'padding:0px',
        defaults: {
            // 应用到所有子面板
            border: false
        },
        // 这里仅仅用几个按钮来示例一种可能的导航场景.
        bbar: [
        {
            id: 'move-prev',
            text: PREV,
            handler: function (btn) {
                navigate(btn.up("panel"), "prev");
            },
            disabled: true
        },
        '->', // 一个长间隔, 使两个按钮分布在两边
        {
            id: 'move-next',
            text: NEXT,
            handler: function (btn) {
                navigate(btn.up("panel"), "next");
            }
        }
        ],
        items: [firstForm, secondForm]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: "通用促銷",
        iconCls: 'icon-user-edit',
        width: 400,
        y: 100,
        layout: 'fit',
        items: [allpan],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        tools: [
             {
                 type: 'close',
                 qtip: CLOSE,
                 handler: function (event, toolEl, panel) {
                     Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                         if (btn == "yes") {
                             editWin.destroy();
                             store.load();
                         }
                         else {
                             return false;
                         }
                     });
                 }
             }],
        listeners: {
            'show': function () {
                if (row) {
                    PromoId = row.data.promo_id;
                    firstForm.getForm().loadRecord(row);
                    secondForm.getForm().loadRecord(row);
                    initForm(row);
                }
                else {
                    firstForm.getForm().reset();
                    secondForm.getForm().reset();
                }
            }
        }
    });
    editWin.show();

    function initForm(Row) {
        Ext.Ajax.request({
            url: '/PromoShare/GetEditData',
            params: {
                PromoId:row.data.promo_id
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                var array = result.data;
                //*裝置*//
                Ext.getCmp('devicename').setValue(array[0].device);
                //*商品編號*//
                if (array[0].product_id != "") {
                    Ext.getCmp('product_id').setValue(array[0].product_id);
                }
                //*會員註冊時間距離*//
                if (array[0].after_user_time != "") {
                    Ext.getCmp('after_user_time').setValue(array[0].after_user_time);
                }
                //*會員首次購買時間距離*//
                if (array[0].after_first_buy_time != "") {
                    Ext.getCmp('after_first_buy_time').setValue(array[0].after_first_buy_time);
                }
                //*會員消費金額*//
                if (array[0].consume_money != "") {
                    Ext.getCmp('consume_money').setValue(array[0].consume_money);
                }
                //*會員購買次數*//
                if (array[0].buy_count != "") {
                    Ext.getCmp('buy_count').setValue(array[0].buy_count);
                }
                //*廠商回饋*//
                if (array[0].vendor_tip != "") {
                    Ext.getCmp('vendor_tip').setValue(array[0].vendor_tip);
                }
                //*bonus*//
                if (array[0].bonus != "") {
                    Ext.getCmp('bonus').setValue(array[0].bonus);
                }
                //*抵用券*//
                if (array[0].voucher != "") {
                    Ext.getCmp('voucher').setValue(array[0].voucher);
                }
                //*運費金額*//
                if (array[0].freight != "") {
                    Ext.getCmp('freight').setValue(array[0].freight);
                }
                //*是否重複送*//
                if (array[0].isrepeat != "") {
                    Ext.getCmp('isrepeat').setValue(array[0].isrepeat);
                }
                //*贈送倍數*//
                if (array[0].multiple != "") {
                    Ext.getCmp('multiple').setValue(array[0].multiple);
                }
                //*url*//
                if (array[0].url != "") {
                    Ext.getCmp('by_url').setValue(array[0].by_url);
                }
                //*圖片*//
                if (array[0].picture != "") {
                    Ext.getCmp('picture').setRawValue(array[0].picture);
                }
                var group_id = array[0].group_id;
                var payType = array[0].payType;
                var site_id = array[0].site_id;
                //*會員羣組賦值*//
                var group_ids = group_id.toString().split(',');
                var combobox1 = Ext.getCmp('group_id');
                var store1 = combobox1.store;
                var arrTemp1 = new Array();
                for (var i = 0; i < group_ids.length; i++) {
                    arrTemp1.push(store1.getAt(store1.find("group_id", group_ids[i])));
                }
                combobox1.setValue(arrTemp1);
                //*付款方式賦值*//
                var payTypes = payType.toString().split(',');
                var combobox2 = Ext.getCmp('payType');
                var store2 = combobox2.store;
                var arrTemp2 = new Array();
                for (var i = 0; i < payTypes.length; i++) {
                    arrTemp2.push(store2.getAt(store2.find("parameterCode", payTypes[i])));
                }
                combobox2.setValue(arrTemp2);
                //*website設定賦值*//
                var site_ids = site_id.toString().split(',');
                var combobox3 = Ext.getCmp('site_id');
                var store3 = combobox3.store;
                var arrTemp3 = new Array();
                for (var i = 0; i < site_ids.length; i++) {
                    arrTemp3.push(store3.getAt(store3.find("Site_Id", site_ids[i])));
                }
                combobox3.setValue(arrTemp3);
            }
        });
    }
    function Tomorrow() {
        var d;
        d = new Date();
        d.setDate(d.getDate() + 1);
        return d;
    }

}

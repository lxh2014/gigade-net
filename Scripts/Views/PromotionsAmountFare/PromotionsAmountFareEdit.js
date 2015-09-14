function editFunction(RowID, PromoAmountFareStore) {

    var currentPanel = 0;
    var promoID = "";
    var prodCateID = "";
    var conditionID = ""; //保存條件id
    var condiCount = 0;
    var boolClass;
    var websiteID = 1;
    var condition_name = "";
    var linkPath = "http://www.gigade100.com/promotion/combo_promotion.php?event_id="; //保存图片链接的地址
    if (RowID != null) {
        promoID = RowID.data.id;
        prodCateID = RowID.data.category_id;
        conditionID = RowID.data.condition_id;
        condition_name = RowID.data.event_type;
        websiteID = RowID.data.site == "" ? 1 : RowID.data.site;

    }
     
    Ext.define("gigade.Brand", {
        extend: 'Ext.data.Model',
        fields: [
        { name: "brand_id", type: "string" },
        { name: "brand_name", type: "string" }]
    });

    //品牌store
    var classBrandStore = Ext.create('Ext.data.Store', {
        model: 'gigade.Brand',
        autoLoad: false,
        autoDestroy: true, //自動銷毀
        remoteSort: false,
        proxy: {
            type: 'ajax',
            url: "/PromotionsMaintain/QueryClassBrand",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'item'
            }
        }
    });

    classBrandStore.on('beforeload', function () {
        Ext.apply(classBrandStore.proxy.extraParams,
       {
           topValue: Ext.getCmp("class_id").getValue()
       });
    });

    VipGroupStore.load();
    ShopClassStore.load();

    if (!RowID) {
        SiteStore.load();
        paymentStore.load();
    }

    var navigate = function (panel, direction) {
        var layout = panel.getLayout();
        if ('next' == direction) {
            if (currentPanel == 0) {//首頁時進行第一步保存
                Ext.getCmp('move-prev').setDisabled(false); //當前為第一個面板時隱藏prev
                if (!RowID) {
                    if (promoID == "") {

                        var event_type = Ext.htmlEncode(activeTypetabPanel.getActiveTab().title);
                        var amount = "0";
                        var quantity = "0";
                        var fare_percent = "0";
                        var off_times = "0";
                        var da1 = Ext.getCmp("dAmount1");
                        var dq2 = Ext.getCmp("dQuantity2");
                        var da3 = Ext.getCmp("dAmount3");
                        var da4 = Ext.getCmp("dAmount4");
                        var dq4 = Ext.getCmp("dQuantity4");
                        var dq3 = Ext.getCmp("dQuantity3");
                        switch (event_type) {
                            case EVENTTYPEONE:
                                da1.allowBlank = false;
                                dq2.allowBlank = true;
                                da3.allowBlank = true;
                                da4.allowBlank = true;
                                dq4.allowBlank = true;
                                dq3.allowBlank = true;
                                if (da1.isValid()) {
                                    amount = Ext.getCmp("dAmount1").getValue();
                                    quantity = "0";
                                    // condition_name = "D1";
                                }
                                break;
                            case EVENTTYPETWO:
                                da1.allowBlank = true;
                                dq2.allowBlank = false;
                                da3.allowBlank = true;
                                da4.allowBlank = true;
                                dq4.allowBlank = true;
                                dq3.allowBlank = true;
                                if (dq2.isValid()) {
                                    amount = "0";
                                    quantity = Ext.getCmp("dQuantity2").getValue();
                                    //condition_name = "D2";
                                }
                                break;
                            case EVENTTYPETHERE:
                                da1.allowBlank = true;
                                dq2.allowBlank = true;
                                da3.allowBlank = false;
                                da4.allowBlank = true;
                                dq4.allowBlank = true;
                                dq3.allowBlank = false;
                                if (da3.isValid() && dq3.isValid()) {
                                    amount = Ext.getCmp("dAmount3").getValue();
                                    quantity = Ext.getCmp("dQuantity3").getValue();
                                    // condition_name = "D3";
                                }
                                break;
                            case EVENTTYPEFOUR:
                                da1.allowBlank = true;
                                dq2.allowBlank = true;
                                da3.allowBlank = true;
                                da4.allowBlank = false;
                                dq4.allowBlank = false;
                                dq3.allowBlank = true;
                                if (da4.isValid() && dq4.isValid()) {
                                    fare_percent = Ext.getCmp("dAmount4").getValue();
                                    off_times = Ext.getCmp("dQuantity4").getValue();

                                    //condition_name = "D4";
                                }
                                break;
                        }
                        var ffrom = firstForm.getForm();
                        if (ffrom.isValid()) {
                            Ext.Ajax.request({
                                url: '/PromotionsAmountFare/FirstSaveFare',
                                method: 'post',
                                params: {
                                    name: Ext.htmlEncode(Ext.getCmp("name").getValue()),
                                    desc: Ext.htmlEncode(Ext.getCmp("event_desc").getValue()),
                                    event_type: Ext.htmlEncode(activeTypetabPanel.getActiveTab().title),
                                    amount: Ext.htmlEncode(amount),
                                    fare_percent: Ext.htmlEncode(fare_percent),
                                    off_times: Ext.htmlEncode(off_times),
                                    vendor_coverage: Ext.htmlEncode(Ext.getCmp('vendor_coverage').getValue()),
                                    // payment: Ext.htmlEncode(Ext.getCmp('payment_name').getValue()),
                                    quantity: Ext.htmlEncode(quantity)
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        promoID = result.id;
                                        prodCateID = result.cateID;
                                        condition_name = result.event_type;

                                        var banner_image = Ext.getCmp("banner_url");
                                        banner_image.setValue(linkPath + GetEventId(condition_name, promoID));
                                        layout[direction]();
                                        currentPanel++;
                                        Ext.getCmp('move-prev').setDisabled(false); //第一次新增時上一步隱藏
                                        if (!layout.getNext()) {
                                            Ext.getCmp('move-next').hide();
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
                        else { return; }
                    }
                    else { //上一步后的下一步
                        var event_type = Ext.htmlEncode(activeTypetabPanel.getActiveTab().title);
                        var da1 = Ext.getCmp("dAmount1");
                        var dq2 = Ext.getCmp("dQuantity2");
                        var da3 = Ext.getCmp("dAmount3");
                        var da4 = Ext.getCmp("dAmount4");
                        var dq4 = Ext.getCmp("dQuantity4");
                        var dq3 = Ext.getCmp("dQuantity3");
                        switch (event_type) {
                            case EVENTTYPEONE:
                                da1.allowBlank = false;
                                dq2.allowBlank = true;
                                da3.allowBlank = true;
                                da4.allowBlank = true;
                                dq4.allowBlank = true;
                                dq3.allowBlank = true;
                                if (da1.isValid()) {
                                    condition_name = "D1";
                                }
                                break;
                            case EVENTTYPETWO:
                                da1.allowBlank = true;
                                dq2.allowBlank = false;
                                da3.allowBlank = true;
                                da4.allowBlank = true;
                                dq4.allowBlank = true;
                                dq3.allowBlank = true;
                                if (dq2.isValid()) {
                                    condition_name = "D2";
                                }
                                break;
                            case EVENTTYPETHERE:
                                da1.allowBlank = true;
                                dq2.allowBlank = true;
                                da3.allowBlank = false;
                                da4.allowBlank = true;
                                dq4.allowBlank = true;
                                dq3.allowBlank = false;
                                if (da3.isValid() && dq3.isValid()) {
                                    condition_name = "D3";
                                }
                                break;
                            case EVENTTYPEFOUR:
                                da1.allowBlank = true;
                                dq2.allowBlank = true;
                                da3.allowBlank = true;
                                da4.allowBlank = false;
                                dq4.allowBlank = false;
                                dq3.allowBlank = true;
                                if (da4.isValid() && dq4.isValid()) {
                                    condition_name = "D4";
                                }
                                break;
                        }
                        var ffrom = firstForm.getForm();
                        if (ffrom.isValid()) {
                            var banner_image = Ext.getCmp("banner_url");
                            banner_image.setValue(linkPath + GetEventId(condition_name, promoID));
                            layout[direction]();
                            currentPanel++;
                            Ext.getCmp('move-prev').setDisabled(false); //第一次新增時上一步隱藏
                            if (!layout.getNext()) {
                                // Ext.getCmp('move-next').setText('保存');
                                Ext.getCmp('move-next').hide();
                            }
                            else {
                                Ext.getCmp('move-next').setText(NEXT_MOVE);
                            }
                        }
                    }
                }
                else {//編輯
                    var event_type = Ext.htmlEncode(activeTypetabPanel.getActiveTab().title);
                    var da1 = Ext.getCmp("dAmount1");
                    var dq2 = Ext.getCmp("dQuantity2");
                    var da3 = Ext.getCmp("dAmount3");
                    var da4 = Ext.getCmp("dAmount4");
                    var dq4 = Ext.getCmp("dQuantity4");
                    var dq3 = Ext.getCmp("dQuantity3");
                    switch (event_type) {
                        case EVENTTYPEONE:
                            da1.allowBlank = false;
                            dq2.allowBlank = true;
                            da3.allowBlank = true;
                            da4.allowBlank = true;
                            dq4.allowBlank = true;
                            dq3.allowBlank = true;
                            if (da1.isValid()) {
                                condition_name = "D1";
                            }
                            break;
                        case EVENTTYPETWO:
                            da1.allowBlank = true;
                            dq2.allowBlank = false;
                            da3.allowBlank = true;
                            da4.allowBlank = true;
                            dq4.allowBlank = true;
                            dq3.allowBlank = true;
                            if (dq2.isValid()) {
                                condition_name = "D2";
                            }
                            break;
                        case EVENTTYPETHERE:
                            da1.allowBlank = true;
                            dq2.allowBlank = true;
                            da3.allowBlank = false;
                            da4.allowBlank = true;
                            dq4.allowBlank = true;
                            dq3.allowBlank = false;
                            if (da3.isValid() && dq3.isValid()) {
                                condition_name = "D3";
                            }
                            break;
                        case EVENTTYPEFOUR:
                            da1.allowBlank = true;
                            dq2.allowBlank = true;
                            da3.allowBlank = true;
                            da4.allowBlank = false;
                            dq4.allowBlank = false;
                            dq3.allowBlank = true;
                            if (da4.isValid() && dq4.isValid()) {
                                condition_name = "D4";
                            }
                            break;
                    }
                    var ffrom = firstForm.getForm();
                    if (ffrom.isValid()) {
                        var banner_image = Ext.getCmp("banner_url");
                        if (RowID.data.category_link_url == "") {
                            banner_image.setValue(linkPath + GetEventId(condition_name, promoID));
                        }
                        else {
                            banner_image.setValue(RowID.data.category_link_url);
                        }
                        layout[direction]();
                        currentPanel++;
                        if (!layout.getNext()) {
                            // Ext.getCmp('move-next').setText('保存');
                            Ext.getCmp('move-next').hide();
                        }
                        else {
                            Ext.getCmp('move-next').setText(NEXT_MOVE);
                        }
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

        //  Ext.getCmp('move-prev').setDisabled(!layout.getPrev());
    };
    var activeTypetabPanel = new Ext.tab.Panel(//分步顯示免運種類
            {
                renderTo: document.body,
                frame: true,
                items: [{
                    title: EVENTTYPEONE,
                    id: 't1',
                    frame: true,
                    items: [{
                        xtype: 'numberfield',
                        id: 'dAmount1',
                        name: 'dAmount1',
                        allowBlank: true,
                        msgTarget: "side",
                        minValue: 0,
                        fieldLabel: ALLMONEY
                    }]
                },
                    {
                        title: EVENTTYPETWO,
                        frame: true,
                        id: 't2',
                        items: [{
                            xtype: 'numberfield',
                            id: 'dQuantity2',
                            name: 'dQuantity2',
                            minValue: 0,
                            allowBlank: true,
                            msgTarget: "side",
                            fieldLabel: FAREQUANTITY
                        }]
                    },
                    {
                        title: EVENTTYPETHERE,
                        id: 't3',
                        frame: true,
                        items: [{
                            xtype: 'numberfield',
                            id: 'dAmount3',
                            name: 'dAmount3',
                            minValue: 0,
                            allowBlank: true,
                            msgTarget: "side",
                            fieldLabel: ALLMONEY
                        },
                        {
                            xtype: 'numberfield',
                            id: 'dQuantity3',
                            name: 'dQuantity3',
                            minValue: 0,
                            msgTarget: "side",
                            allowBlank: true,
                            fieldLabel: FAREQUANTITY
                        }]
                    },
                    {//0為免運費
                        title: EVENTTYPEFOUR,
                        id: 't4',
                        frame: true,
                        items: [{
                            xtype: 'numberfield',
                            id: 'dAmount4',
                            name: 'dAmount4',
                            labelWidth: 125,
                            msgTarget: "side",
                            minValue: 0,
                            fieldLabel: FATEAMOUNT
                        },
                         {//0為無限制
                             xtype: 'numberfield',
                             id: 'dQuantity4',
                             name: 'dQuantity4',
                             labelWidth: 125,
                             minValue: 0,
                             msgTarget: "side",
                             fieldLabel: FARETIMES
                         }
                        ]
                    }
                ]
            });
    var firstForm = Ext.widget('form',
           {
               id: 'editFrm1',
               frame: true,
               plain: true,
               defaultType: 'textfield',
               layout: 'anchor',
               defaults: { anchor: "95%", msgTarget: "side" },
               items: [
                {
                    xtype: 'textfield',
                    id: 'id',
                    name: 'id',
                    hidden: true
                },
                {
                    fieldLabel: ACTIVENAME,
                    xtype: 'textfield',
                    padding: '10 0 5 0',
                    id: 'name',
                    name: 'name',
                    allowBlank: false,
                    submitValue: true,
                    labelWidth: 80
                },
                {
                    fieldLabel: ACTIVEDESC,
                    xtype: 'textfield',
                    padding: '10 0 5 0',
                    id: 'event_desc',
                    name: 'event_desc',
                    allowBlank: false,
                    submitValue: true,
                    labelWidth: 80
                },
                {
                    xtype: "numberfield",
                    fieldLabel: VENDORCOVERAGE,
                    id: 'vendor_coverage',
                    name: 'vendor_coverage',
                    allowBlank: false,
                    margin: '5 0 5 0',
                    minValue: 0,
                    value: 0,
                    regex: /^[-+]?([1-9]\d*|0)$/,
                    regexText: VENDERTIP,
                    submitValue: true

                },
                  {
                      xtype: 'fieldset',
                      title: ACTIVETYPE,
                      defaultType: 'textfield',
                      layout: 'anchor',
                      defaults: {
                          anchor: '100%',
                          msgTarget: "side"
                      },
                      items: [activeTypetabPanel]
                  }

               ]
           });
    var thirdForm = Ext.widget('form',
           {
               id: 'editFrm',
               frame: true,
               plain: true,
               defaultType: 'textfield',
               height: 480,
               autoScroll: true,
               layout: 'anchor',
               url: '/PromotionsAmountFare/SecondSaveFare',
               defaults: { anchor: "95%", msgTarget: "side" },
               items: [
                {
                    xtype: 'radiogroup',
                    fieldLabel: ISBANNERURL,
                    id: 'url_by',
                    colName: 'url_by',
                    name: 'url_by',
                    defaults: {
                        name: 'Url_By'
                    },
                    columns: 2,
                    vertical: true,
                    items: [
                               {
                                   id: 'bt1',
                                   boxLabel: YES,
                                   inputValue: '1',
                                   width: 100,
                                   listeners: {
                                       change: function (radio, newValue, oldValue) {
                                           var banner_url = Ext.getCmp("banner_url");
                                           var photo = Ext.getCmp("photo");
                                           var allClass = Ext.getCmp("allClass");
                                           if (newValue) {
                                               banner_url.setValue(linkPath + GetEventId(condition_name, promoID));
                                               banner_url.allowBlank = false;
                                               banner_url.show();
                                               photo.allowBlank = false;
                                               photo.show();
                                               allClass.setValue(false);
                                               allClass.setDisabled(true);
                                           }
                                       }
                                   }
                               },
                               {
                                   id: 'bt2',
                                   boxLabel: NO,
                                   checked: true,
                                   inputValue: '0',
                                   listeners: {
                                       change: function (radio, newValue, oldValue) {
                                           var banner_url = Ext.getCmp("banner_url");
                                           var photo = Ext.getCmp("photo");
                                           var allClass = Ext.getCmp("allClass");
                                           if (newValue) {
                                               banner_url.setValue("0");
                                               banner_url.allowBlank = true;
                                               banner_url.setValue("");
                                               banner_url.isValid();
                                               banner_url.hide();

                                               photo.setValue("0");
                                               photo.allowBlank = true;
                                               photo.setValue("");
                                               photo.isValid();
                                               photo.hide();
                                               allClass.setDisabled(false);

                                           }
                                       }
                                   }
                               }
                    ]
                },
               {
                   xtype: 'textfield',
                   fieldLabel: SELFURL,
                   name: 'category_link_url',
                   id: 'banner_url',
                   submitValue: true,
                   vtype: 'url',
                   allowBlank: true,
                   hidden: true
               },
               {//專區Banner
                   xtype: 'filefield',
                   name: 'photo',
                   id: 'photo',
                   fieldLabel: BANNERIMG,
                   msgTarget: 'side',
                   buttonText: SELECT_IMG,
                   submitValue: true,
                   allowBlank: true,
                   hidden: true,
                   fileUpload: true
               },
               { //會員群組和會員條件二擇一
                   xtype: 'fieldcontainer',
                   fieldLabel: VIPGROUP,
                   combineErrors: true,
                   layout: 'hbox',
                   defaults: {
                       width: 120,
                       hideLabel: true
                   },
                   items: [
                     {
                         xtype: 'radiofield',
                         name: 'us',
                         inputValue: "u_group",
                         id: "us1",
                         checked: true,
                         listeners: {
                             change: function (radio, newValue, oldValue) {
                                 var com_group = Ext.getCmp("group_name");
                                 var btn_group = Ext.getCmp("condi_set");
                                 if (newValue) {
                                     btn_group.setDisabled(true);
                                     com_group.setValue("0");
                                     com_group.setDisabled(false);
                                     com_group.allowBlank = false;
                                     Ext.getCmp("userShow").hide();
                                 }
                             }
                         }
                     },
                     {
                         xtype: 'combobox', //會員群組
                         editable: false,
                         hidden: false,
                         id: 'group_name',
                         name: 'group_name',
                         hiddenName: 'group_name',
                         colName: 'group_name',
                         store: VipGroupStore,
                         displayField: 'group_name',
                         submitValue: true,
                         valueField: 'group_id',
                         typeAhead: true,
                         forceSelection: false,
                         lastQuery: '',
                         value: "0"

                     }
                   ]
               },
               {
                   xtype: 'fieldcontainer',
                   fieldLabel: VIPCONDITION,
                   combineErrors: true,
                   layout: 'hbox',
                   defaults: {
                       width: 120,
                       hideLabel: true
                   },
                   items: [
                     {
                         xtype: 'radiofield',
                         name: 'us',
                         inputValue: "u_groupset",
                         id: "us2",
                         listeners: {
                             change: function (radio, newValue, oldValue) {
                                 var com_group = Ext.getCmp("group_name");
                                 var btn_group = Ext.getCmp("condi_set");
                                 if (newValue) {
                                     com_group.setValue("0");
                                     com_group.allowBlank = true;
                                     com_group.setValue("");
                                     com_group.isValid();
                                     btn_group.setDisabled(false);
                                     com_group.setDisabled(true);
                                     if (condition_id != "" && condition_id != 0) {
                                         Ext.getCmp("userShow").show();
                                     }
                                 }
                             }
                         }
                     },
                     {
                         xtype: 'button',
                         text: CONDINTION,
                         disabled: true,
                         width: 120,
                         id: 'condi_set',
                         colName: 'condi_set',
                         handler: function () {
                             if (conditionID != "") {
                                 showUserSetForm(condition_name, conditionID, "userInfo");
                             } else {
                                 showUserSetForm(condition_name, condition_id, "userInfo");
                             }
                         }
                     }]
               },
               {
                   xtype: 'fieldset',
                   title: USERDETAIL,
                   defaultType: 'textfield',
                   id: 'userShow',
                   layout: 'anchor',
                   hidden: true,
                   defaults: {
                       anchor: '100%'
                   },
                   items: [
                    {//顯示保存的條件設定
                        xtype: 'textareafield',
                        name: 'textarea1',
                        id: 'userInfo',
                        readOnly: true,
                        anchor: '100%',
                        value: ShowConditionData(conditionID, "userInfo"),
                        listeners: {
                            change: function (textarea, newValue, oldValue) {
                                var textArea = Ext.getCmp("userInfo");
                                var userShow = Ext.getCmp("userShow");
                                if (newValue != "" && oldValue != "") {
                                    userShow.show();
                                }
                            }
                        }
                    }]
               },
               {
                   xtype: 'combobox', //Wibset
                   fieldLabel: WEBSITESET,
                   id: 'site',
                   name: 'site_name',
                   hiddenName: 'site_name',
                   colName: 'site',
                   allowBlank: false,
                   editable: false,
                   store: SiteStore,
                   displayField: 'Site_Name',
                   valueField: 'Site_Id',
                   queryMode: 'local',
                   multiSelect: true, //支持多選
                   typeAhead: true,
                   forceSelection: false,
                   emptyText: SELECT,
                   listeners: {
                       select: function (a, b, c) {
                           websiteID = Ext.getCmp('site').getValue();
                       }
                   }
               },
                   {//付款方式
                       xtype: 'combobox',
                       fieldLabel: PAYTYPE,
                       id: 'payment_name',
                       name: 'payment_name',
                       allowBlank: false,
                       editable: false,
                       typeAhead: true,
                       queryMode: 'local',
                       multiSelect: true, //支持多選
                       forceSelection: false,
                       store: paymentStore,
                       displayField: 'parameterName',
                       valueField: 'parameterCode',
                       emptyText: SELECT
                   },
               {
                   xtype: 'combobox', //class_id
                   allowBlank: true,
                   fieldLabel: SHOPCLASS,
                   editable: false,
                   id: 'class_id',
                   name: 'class_id',
                   hiddenName: 'class_id',
                   colName: 'class_id',
                   store: ShopClassStore,
                   displayField: 'class_name',
                   valueField: 'class_id',
                   typeAhead: true,
                   forceSelection: false,
                   emptyText: SELECT,
                   listeners: {
                       "select": function (combo, record) {
                           var z = Ext.getCmp("brand_id");
                           z.clearValue();
                           classBrandStore.removeAll();
                           boolClass = true;
                           z.setDisabled(false);
                       }
                   }
               },
               {
                   xtype: 'combobox', //banner_id
                   allowBlank: true,
                   fieldLabel: BLANDNAME,
                   editable: true,
                   forceSelection: true,
                   queryMode: 'local',
                   id: 'brand_id',
                   name: 'brand_name',
                   hiddenname: 'brand_name',
                   colName: 'brand_name',
                   disabled: true,
                   store: classBrandStore,
                   displayField: 'brand_name',
                   valueField: 'brand_id',
                   typeAhead: true,
                   forceSelection: false,
                   emptyText: SELECT,
                   listeners: {
                       beforequery: function (qe) {
                           if (boolClass) {
                               delete qe.combo.lastQuery;
                               classBrandStore.load({
                                   params: {
                                       topValue: Ext.getCmp("class_id").getValue()
                                   }
                               });
                               boolClass = false;
                           }
                       },
                       'blur': function () {
                           var o = classBrandStore.data.items;
                           if (document.getElementsByName('brand_name')[0].value != Ext.getCmp('brand_id').getValue()) {
                               document.getElementsByName('brand_name')[0].value = classBrandStore.getAt(0).get('brand_id');
                           }
                       }
                   }

               },
               //促銷商品類別 
                    {
                        xtype: 'form', //類別設定
                        hidden: false,
                        layout: 'hbox',
                        border: 0,
                        frame: true,
                        id: 'p_sortpanl',
                        colName: 'p_sortpanl',
                        items: [
                          {
                              xtype: 'label',
                              text: PROSORT,
                              id: 'p_sort',
                              width: 100
                              // margin: '0 8 0 0'
                          }, {
                              xtype: 'button',
                              text: CONDINTION,
                              width: 120,
                              id: 'p_condi',
                              colName: 'p_condi',
                              name: 'p_condi',
                              handler: function () {
                                  var sclass = Ext.getCmp('class_id');
                                  var brand = Ext.getCmp('brand_id');
                                  var allClass = Ext.getCmp('allClass');
                                  sclass.setValue("");
                                  brand.setValue("");
                                  brand.setDisabled(true);
                                  allClass.setValue(false);
                                  condiCount++;
                                  var regex = /^([0-9]+,)*[0-9]+$/; //類似1|1，3，5|12,4
                                  if (!regex.test(websiteID)) {
                                      websiteID = SiteStore.getAt(0).get('Site_id')
                                  }
                                  PromationMationShow(prodCateID, websiteID);
                              }
                          },
                           {
                               xtype: 'checkbox',
                               boxLabel: ALLSHOP,
                               id: 'allClass',
                               name: 'allClass',
                               margin: '0 0 0 20',
                               inputValue: '1',
                               listeners: {
                                   change: function (checkbox, newValue, oldValue) {
                                       var sclass = Ext.getCmp('class_id');
                                       var brand = Ext.getCmp('brand_id');
                                       var btn_condi = Ext.getCmp('p_condi');
                                       if (newValue) {
                                           sclass.setValue("");
                                           brand.setValue("");
                                           sclass.setDisabled(true);
                                           brand.setDisabled(true);
                                           btn_condi.setDisabled(true);
                                       }
                                       else {
                                           sclass.setDisabled(false);
                                           btn_condi.setDisabled(false);
                                       }
                                   }
                               }

                           }
                        ]
                    },
               {
                   xtype: 'combobox',
                   fieldLabel: DELIVERYSTORE,
                   store: DeliverStore,
                   id: 'delivery_store',
                   name: 'delivery_store',
                   allowBlank: false,
                   editable: false,
                   margin: '5 0 5 0',
                   queryMode: 'local',
                   displayField: 'deliver_name',
                   valueField: 'deliver_id',
                   typeAhead: true,
                   forceSelection: false,
                   emptyText: SELECT
               },
               {
                   xtype: 'combobox',
                   fieldLabel: YSCLASS,
                   store: YunsongStore,
                   queryMode: 'local',
                   allowBlank: false,
                   editable: false,
                   id: 'typeName',
                   name: 'typeName',
                   hiddenName: 'typeName',
                   colName: 'typeName',
                   displayField: 'type_name',
                   valueField: 'type_id',
                   typeAhead: true,
                   forceSelection: false,
                   emptyText: SELECT
               },

               {
                   xtype: 'radiogroup',
                   hidden: false,
                   id: 'deviceName',
                   name: 'deviceName',
                   fieldLabel: DEVICE,
                   colName: 'deviceName',
                   width: 400,
                   defaults: {
                       name: 'deviceName',
                       margin: '0 8 0 0'
                   },
                   columns: 3,
                   vertical: true,
                   items: [
            { boxLabel: DEVICE_1, id: 'bufen', inputValue: '0', checked: true },
            { boxLabel: DEVICE_2, id: 'pc', inputValue: '1' },
            { boxLabel: DEVICE_3, id: 'mobilepad', inputValue: '4' }
                   ]
               },
                {
                    xtype: "datetimefield",
                    fieldLabel: BEGINTIME,
                    editable: false,
                    id: 'start_time',
                    name: 'start_time',
                    anchor: '95%',
                    format: 'Y-m-d H:i:s',
                    width: 150,
                    allowBlank: false,
                    submitValue: true,
                    value: Tomorrow(),
                    time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("start_time");
                            var end = Ext.getCmp("end_time");
                            if (end.getValue() == null) {
                                end.setValue(setNextMonth(start.getValue(), 1));
                            }
                            else if (end.getValue() < start.getValue()) {
                                Ext.Msg.alert(INFORMATION, DATA_TIP);
                                start.setValue(setNextMonth(end.getValue(), -1));
                            }
                        }
                    }
                },
                      {
                          xtype: "datetimefield",
                          fieldLabel: ENDTIME,
                          editable: false,
                          id: 'end_time',
                          anchor: '95%',
                          name: 'end_time',
                          format: 'Y-m-d H:i:s',
                          width: 150,
                          allowBlank: false,
                          submitValue: true, //
                          value: setNextMonth(Tomorrow(), 1),
                          listeners: {
                              select: function (a, b, c) {
                                  var start = Ext.getCmp("start_time");
                                  var end = Ext.getCmp("end_time");
                                  if (start.getValue() != "" && start.getValue() != null) {
                                      if (end.getValue() < start.getValue()) {
                                          Ext.Msg.alert(INFORMATION, DATA_TIP);
                                          end.setValue(setNextMonth(start.getValue(), 1));
                                      }

                                  }
                                  else {
                                      start.setValue(setNextMonth(end.getValue(), -1));
                                  }
                              }
                          }
                      }

               ],
               buttons: [{
                   text: SAVE,
                   formBind: true,
                   disabled: false,
                   handler: function () {
                      
                       var event_type = Ext.htmlEncode(activeTypetabPanel.getActiveTab().title);
                       var amount = "";
                       var quantity = "";
                       var fare_percent = "0";
                       var off_times = "0";
                       switch (event_type) {
                           case EVENTTYPEONE:
                               if (Ext.getCmp("dAmount1").isValid()) {
                                   quantity = "0";
                                   amount = Ext.getCmp("dAmount1").getValue();
                               }
                               break;
                           case EVENTTYPETWO:
                               if (Ext.getCmp("dQuantity2").isValid()) {
                                   amount = "0";
                                   quantity = Ext.getCmp("dQuantity2").getValue();
                               }
                               break;
                           case EVENTTYPETHERE:
                               if (Ext.getCmp("dAmount3").isValid() && Ext.getCmp("dQuantity3").isValid()) {
                                   amount = Ext.getCmp("dAmount3").getValue();
                                   quantity = Ext.getCmp("dQuantity3").getValue();
                               }
                               break;
                           case EVENTTYPEFOUR:
                               if (Ext.getCmp("dAmount4").isValid() && Ext.getCmp("dQuantity4").isValid()) {
                                   fare_percent = Ext.getCmp("dAmount4").getValue();
                                   off_times = Ext.getCmp("dQuantity4").getValue();
                                   amount = "0";
                                   quantity = "0";
                               }
                               break;
                       }
                       var form = this.up('form').getForm();
                       if (form.isValid()) {
                           if (!Ext.getCmp('allClass').getValue() && (condiCount == 0 && RowID == null) && (Ext.getCmp('brand_id').getValue() == "" || Ext.getCmp('brand_id').getValue() == null || Ext.getCmp('brand_id').getValue() == "0")) {
                               Ext.Msg.alert(INFORMATION, CONDITIONTIP);
                               return;
                           }
                           if (RowID) {
                               if (RowID.data.product_id == 999999)//編輯時當原數據是全館時判斷是否點擊條件設定或者設置品牌
                               {
                                   if (condiCount == 0 && (Ext.getCmp('brand_id').getValue() == "" || Ext.getCmp('brand_id').getValue() == null || Ext.getCmp('brand_id').getValue() == "0") && !Ext.getCmp('allClass').getValue()) {
                                       Ext.Msg.alert(INFORMATION, CONDITIONTIP);
                                       return;
                                   }
                               }
                           }

                           if (condition_id == 0 && Ext.getCmp("group_name").getValue() == "") {
                               Ext.Msg.alert(INFORMATION, USERCONDITIONERROR);
                               return;
                           }
                           this.disable();
                           form.submit({
                               params: {
                                   isEdit: RowID,
                                   rowid: promoID,
                                   cateid: prodCateID,
                                   name: Ext.getCmp("name").getValue(),
                                   desc: Ext.getCmp("event_desc").getValue(),
                                   event_type: Ext.htmlEncode(activeTypetabPanel.getActiveTab().title),
                                   amount: Ext.htmlEncode(amount),
                                   quantity: Ext.htmlEncode(quantity),
                                   fare_percent: Ext.htmlEncode(fare_percent),
                                   off_times: Ext.htmlEncode(off_times),
                                   banner_url: Ext.htmlEncode(Ext.getCmp("banner_url").getValue()),
                                   event_type: Ext.htmlEncode(activeTypetabPanel.getActiveTab().title),
                                   group_id: Ext.htmlEncode(Ext.getCmp('group_name').getValue()),
                                   sclass_id: Ext.htmlEncode(Ext.getCmp('class_id').getValue()),
                                   allClass: Ext.htmlEncode(Ext.getCmp('allClass').getValue()),
                                   sbrand_id: document.getElementsByName('brand_name')[0].value,
                                   condition_id: condition_id,
                                   type: Ext.htmlEncode(Ext.getCmp('typeName').getValue()),
                                   deliver: Ext.htmlEncode(Ext.getCmp('delivery_store').getValue()),
                                   payment: Ext.htmlEncode(Ext.getCmp('payment_name').getValue()),
                                   start_time: Ext.htmlEncode(Ext.getCmp('start_time').getRawValue()),
                                   end_time: Ext.htmlEncode(Ext.getCmp('end_time').getRawValue()),
                                   vendor_coverage: Ext.htmlEncode(Ext.getCmp('vendor_coverage').getValue()),
                                   site: Ext.htmlEncode(Ext.getCmp('site').getValue())
                               },
                               success: function (form, action) {
                                   var result = Ext.decode(action.response.responseText);
                                   if (result.success) {
                                       Ext.Msg.alert(INFORMATION, SUCCESS);
                                       PromoAmountFareStore.load();
                                       editWin.close();
                                   }
                                   else {
                                       Ext.Msg.alert(INFORMATION, FAILURE);
                                   }
                               },
                               failure: function (form, action) {
                                   var result = Ext.decode(action.response.responseText);
                                   if (result.msg.toString() == "1") {
                                       Ext.Msg.alert(INFORMATION, PHOTOSIZE);
                                   }
                                   else if (result.msg.toString() == "2") {
                                       Ext.Msg.alert(INFORMATION, PHOTOTYPE);
                                   }
                                   else if (result.msg.toString() == "3") {
                                       Ext.Msg.alert(INFORMATION, USERCONDIERROR);
                                   }
                                   else {
                                       Ext.Msg.alert(INFORMATION, FAILURE);
                                   }
                               }
                           });
                       }
                   }
               }]
           });
    var allpan = new Ext.form.Panel({
        width: 400,
        layout: 'card',
        border: 0,
        bodyStyle: 'padding:0px',
        defaults: {
            border: false
        },
        bbar: [
            {
                id: 'move-prev',
                text: PREV_MOVE,
                handler: function (btn) {
                    navigate(btn.up("panel"), "prev");
                },
                disabled: true
            },
            '->', // 一个长间隔, 使两个按钮分布在两边
            {
                id: 'move-next',
                text: NEXT_MOVE,
                margin: '0 3 0 0',
                handler: function (btn) {
                    navigate(btn.up("panel"), "next");
                }
            }
        ],
        items: [firstForm, thirdForm]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: EVENTTYPETHERE,
        //        id: 'editWin',
        iconCls: RowID ? "icon-user-edit" : "icon-user-add",
        width: 400,
        y: 100,
        // height: document.documentElement.clientHeight * 360 / 783,
        layout: 'fit',
        constrain: true,
        items: [allpan],
        closeAction: 'destroy',
        modal: true,
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSEFORM,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         editWin.destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function () {
                if (RowID) {
                    firstForm.getForm().loadRecord(RowID);
                    thirdForm.getForm().loadRecord(RowID);
                    initForm(RowID);
                }
                else {
                    firstForm.getForm().reset();
                    thirdForm.getForm().reset();
                }
            }
        }
    });
    editWin.show();

    function initForm(row) {

        Ext.getCmp('brand_id').setRawValue(row.data.brand_name);
        if (row.data.class_id == "0") {
            Ext.getCmp('class_id').setValue("");
        }

        if (row.data.class_id != 0 && row.data.brand_id != 0) {
            Ext.getCmp('brand_id').setDisabled(false);
            classBrandStore.load({
                params: {
                    topValue: Ext.getCmp("class_id").getValue()
                }
            });
        }
        switch (row.data.event_type_name) {
            case EVENTTYPEONE:
                activeTypetabPanel.setActiveTab("t1");
                Ext.getCmp("dAmount1").setValue(row.data.amount);
                break;
            case EVENTTYPETWO:
                activeTypetabPanel.setActiveTab("t2");
                Ext.getCmp("dQuantity2").setValue(row.data.quantity);
                break;
            case EVENTTYPETHERE:
                activeTypetabPanel.setActiveTab("t3");
                Ext.getCmp("dAmount3").setValue(row.data.amount);
                Ext.getCmp("dQuantity3").setValue(row.data.quantity);
                break;
            case EVENTTYPEFOUR:
                activeTypetabPanel.setActiveTab("t4");
                Ext.getCmp("dAmount4").setValue(row.data.fare_percent);
                Ext.getCmp("dQuantity4").setValue(row.data.off_times);
                break;
        }
        if (row.data.url_by == 0) {
            Ext.getCmp("bt2").setValue(true);
            Ext.getCmp("banner_url").setValue("");

        }
        else if (row.data.url_by == 1) {
            Ext.getCmp("bt1").setValue(true);
            var img = row.data.banner_image.toString();
            var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
            Ext.getCmp('photo').setRawValue(imgUrl);
        }
        if (row.data.typeName == "") {
            Ext.getCmp("typeName").setValue(0);
        }

        if (row.data.product_id == 999999) {
            Ext.getCmp("allClass").setValue(true);
        }
        if (row.data.payment_name == "") {
            Ext.getCmp("payment_name").setValue(DEVICE_1); //不分
        }

        switch (row.data.deviceName) {
            case DEVICE_2:
                Ext.getCmp("pc").setValue(true);
                break;
            case DEVICE_3:
                Ext.getCmp("mobilepad").setValue(true);
                break;
            default:
                Ext.getCmp("bufen").setValue(true);
                break;
        }

        if (row.data.condition_id != 0) {
            Ext.getCmp('us1').setValue(false);
            Ext.getCmp('us2').setValue(true);
        }
        else {
            Ext.getCmp('us1').setValue(true);
            Ext.getCmp('us2').setValue(false);
            if (row.data.group_name == "") {
                Ext.getCmp('group_name').setValue(BUFEN);
            }
        }
        //修改時對site賦值
        //修改時對site賦值
        SiteStore.load({
            callback: function () {
                var siteIDs = row.data.site.toString().split(',');
                var arrTemp = new Array();
                for (var i = 0; i < siteIDs.length; i++) {
                    arrTemp.push(SiteStore.getAt(SiteStore.find("Site_Id", siteIDs[i])));
                }
                Ext.getCmp('site').setValue(arrTemp);

            }
        })

        //修改時對site賦值
        paymentStore.load({
            callback: function () {
                var payTypeIDs = row.data.payment_code.toString().split(',');
                var payArr = new Array();
                for (var i = 0; i < payTypeIDs.length; i++) {
                    payArr.push(paymentStore.getAt(paymentStore.find("parameterCode", payTypeIDs[i])));
                }
                Ext.getCmp('payment_name').setValue(payArr);

            }
        })

    }
    function GetEventId(ctype, cid) {
        var sResult = ctype;
        if (cid.toString().length < 6) {
            for (var i = 0; i < 6 - cid.toString().length; i++) {
                sResult += "0";
            }
        }
        sResult += cid;
        return sResult;
    }
}


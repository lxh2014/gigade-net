
function editFunction(RowID, PromoAmountGiftStore) {

    var conditionID = ""; //保存條件id
    var event_type = "";
    var condiCount = 0;
    var boolClass;
    var condition_name = "";
    var linkPath = "http://www.gigade100.com/promotion/combo_promotion.php?event_id="; //保存图片链接的地址
    var currentPanel = 0;
    var promoID = ""; 
    var prodCateID = "";
    var websiteID = 1;
    ShopClassStore.load();
    VipGroupStore.load();
    YunsongStore.load();
    if (RowID != null) {
        promoID = RowID.data.id;
        prodCateID = RowID.data.category_id;
        websiteID = RowID.data.site == "" ? 1 : RowID.data.site;
        conditionID = RowID.data.condition_id;
    }
    else {
        paymentStore.load();
        SiteStore.load();
    }
    Ext.define("gigade.UseEndModel", {
        extend: 'Ext.data.Model',
        fields: [
        { name: "value", type: "int" }]
    });
    var UseEndStore = Ext.create('Ext.data.Store', {
        model: 'gigade.UseEndModel',
        autoLoad: true,
        data: [
        { value: '0' },
        { value: '1' },
        { value: '2' },
        { value: '3' },
        { value: '4' },
        { value: '5' },
        { value: '6' },
        { value: '7' },
        { value: '8' },
        { value: '9' },
        { value: '10' },
        { value: '11' },
        { value: '12' }
        ]
    });

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
    //非專區品牌store
    var classBrandStore_no = Ext.create('Ext.data.Store', {
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
    classBrandStore_no.on('beforeload', function () {
        Ext.apply(classBrandStore_no.proxy.extraParams,
       {
           topValue: Ext.htmlEncode(Ext.getCmp("no_class_id").getValue())

       });
    });
    //送禮類別Model
    Ext.define("gigade.GiftTypeModel", {
        extend: 'Ext.data.Model',
        fields: [
        { name: "type_id", type: "string" },
        { name: "type_name", type: "string" }]
    });
    //送禮類別store
    var GiftTypeStore = Ext.create('Ext.data.Store', {
        model: 'gigade.GiftTypeModel',
        autoLoad: true,
        data: [
        { type_id: '0', type_name: PRODUCT },
        { type_id: '1', type_name: CHANCE },
        { type_id: '2', type_name: BONUS_ONE },
        { type_id: '3', type_name: BONUS_TWO }
        ]
    });
    var navigate = function (panel, direction) {
        var layout = panel.getLayout();
        var move = Ext.getCmp('move-next').text;
        var oldevent_type_G;
        if ('next' == direction) {
            if (currentPanel == 0) {
                var formf = firstForm.getForm();
                if (!RowID) {
                    if (formf.isValid()) {
                        if (Ext.getCmp("amount").getValue() != 0 && Ext.getCmp("amount").getValue() != null) {
                            event_type = AMOUNTGIFT;
                            condition_name = "G1";

                        } else if (Ext.getCmp("quantity").getValue() != 0 && Ext.getCmp("quantity").getValue() != null) {
                            event_type = QUANTITYGIFT;
                            condition_name = "G2";
                        }
                        Ext.Ajax.request({
                            url: '/PromotionsAmountGift/FirstSaveGift',
                            method: 'post',
                            params: {
                                name: Ext.getCmp("name").getValue(),
                                desc: Ext.getCmp("event_desc").getValue(),
                                amount: Ext.getCmp("amount").getValue(),
                                quantity: Ext.getCmp("quantity").getValue(),
                                vendor_coverage: Ext.htmlEncode(Ext.getCmp('vendor_coverage').getValue()),
                                event_type_name: event_type
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    promoID = result.id;
                                    prodCateID = result.cateID;
                                    // event_type = result.event_type;
                                    //event_id = result.event_id;
                                    Ext.getCmp('move-prev').setDisabled(false);
                                    Ext.getCmp('move-prev').show();
                                    var banner_image = Ext.getCmp("banner_url");
                                    banner_image.setValue(linkPath + GetEventId(condition_name, promoID));
                                    layout[direction]();
                                    currentPanel++;
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
                }
                else {
                    if (formf.isValid()) {
                        if (condition_name == "") {
                            oldevent_type_G = RowID.data.event_type;
                        } else {
                            oldevent_type_G = condition_name;
                        }
                        if (Ext.getCmp("amount").getValue() != 0 && Ext.getCmp("amount").getValue() != null) {
                            event_type = AMOUNTGIFT;
                            condition_name = "G1";

                        } else if (Ext.getCmp("quantity").getValue() != 0 && Ext.getCmp("quantity").getValue() != null) {
                            event_type = QUANTITYGIFT;
                            condition_name = "G2";
                        }

                        var banner_image = Ext.getCmp("banner_url");
                        if (oldevent_type_G == condition_name) {
                            if (RowID != null && RowID != "" && RowID.data.category_link_url != "") {
                                banner_image.setValue(RowID.data.category_link_url);
                            }
                        } else {
                            banner_image.setValue(linkPath + GetEventId(condition_name, promoID));
                        }


                        layout[direction]();
                        currentPanel++;
                    }
                }
            } else {
                var forms = secondForm.getForm();
                if (forms.isValid()) {

                    layout[direction]();
                    currentPanel++;
                    Ext.getCmp('move-next').hide();
                }
            }

        } else {
            layout[direction]();
            currentPanel--;
            Ext.getCmp('move-next').show();

        }
        Ext.getCmp('move-prev').setDisabled(!layout.getPrev());
    };
    var activeTypetabPanel = new Ext.tab.Panel(
            {
                renderTo: document.body,
                frame: true,
                items: [
                {
                    title: PRODUCT,
                    frame: true,
                    id: 'product',
                    items: [
                    {
                        xtype: 'numberfield',
                        name: 'proGift',
                        id: 'proGift',
                        minLength: 5,
                        maxLength: 6,
                        fieldLabel: PRODFIVEORSIX,
                        minValue: 10000,
                        macValue: 999999,
                        fieldLabel: PRODFIVEORSIX,
                        listeners: {
                            change: function (field, value) {
                                if (value == 0) {
                                    field.setMinValue(0);
                                }
                                else {
                                    field.setMinValue(10000);
                                }

                            }
                        }
                    },
                    {
                        xtype: 'numberfield',
                        name: 'gift_product_number',
                        id: 'gift_product_number',
                        maxLength: 6,
                        fieldLabel: '贈品數量',
                        minValue: 0,
                        maxValue: 999999,
                        fieldLabel: '贈品數量',
                        listeners: {
                            change: function (field, value) {
                                if (value == 0) {
                                    field.setMinValue(0);
                                }
                            }
                        }
                    }
                    ],
                    listeners: {
                        activate: function (tab, oldtab) {
                            if (oldtab.id != undefined) {
                                Ext.getCmp("us_s1").reset();
                                Ext.getCmp("us_startChoice").reset();
                                Ext.getCmp("us_s2").reset();
                                Ext.getCmp("us_startChoice").reset();

                                Ext.getCmp("use_endMonth").reset();
                                Ext.getCmp("us_e1").reset();
                                Ext.getCmp("use_endChoice").reset();
                                Ext.getCmp("us_e2").reset();

                                Ext.getCmp("us_bs1").reset();
                                Ext.getCmp("us_bstartChoice").reset();
                                Ext.getCmp("us_bs2").reset();
                                Ext.getCmp("us_bstartChoice").reset();

                                Ext.getCmp("deduct_welfare").reset();
                                Ext.getCmp("use_bendMonth").reset();
                                Ext.getCmp("us_be1").reset();
                                Ext.getCmp("use_bendChoice").reset();
                                Ext.getCmp("us_be2").reset();
                            }
                        }
                    }
                },
                {
                    title: CHANCE, //機會
                    id: 'chanceGift',
                    frame: true,
                    items: [{
                        xtype: 'radiogroup',
                        hidden: false,
                        id: 'chanceXuhao',
                        name: 'chanceXuhao',
                        fieldLabel: ISXUHAO,
                        colName: 'chanceXuhao',
                        width: 400,
                        defaults: {
                            margin: '0 8 0 0'
                        },
                        columns: 2,
                        vertical: true,
                        items: [
                                {
                                    boxLabel: YES,
                                    name: 'ChanceIsXuhao',
                                    id: 'yesXH',
                                    inputValue: "1",
                                    checked: true
                                },
                                {
                                    boxLabel: NO,
                                    name: 'ChanceIsXuhao',
                                    id: 'noXH',
                                    inputValue: "0"
                                    //checked: true
                                }]

                    },
                      {
                          xtype: 'radiogroup',
                          hidden: false,
                          id: 'IsRepeatXuhao',
                          name: 'IsRepeatXuhao',
                          fieldLabel: ISREPEATXUHAO,
                          colName: 'IsRepeatXuhao',
                          width: 400,
                          defaults: {
                              margin: '0 8 0 0'
                          },
                          columns: 2,
                          vertical: true,
                          items: [
                                {
                                    boxLabel: YES,
                                    name: 'rIsRepeatXuhao',
                                    id: 'yesRXH',
                                    inputValue: "1",
                                    checked: true
                                },
                                {
                                    boxLabel: NO,
                                    name: 'rIsRepeatXuhao',
                                    id: 'noRXH',
                                    inputValue: "0"
                                    //checked: true
                                }]

                      },
                    {
                        xtype: 'fieldset',
                        flex: 1,
                        layout: 'anchor',
                        items: [
                        {
                            xtype: 'fieldcontainer',
                            combineErrors: true,
                            layout: 'hbox',
                            defaults: {
                                flex: 1,
                                hideLabel: true
                            },
                            items: [
                            {
                                xtype: 'checkbox',
                                boxLabel: CREATEXUHAO,
                                margin: '2 0 0 0',
                                id: 'IsXuhao',
                                name: 'IsXuhao',
                                inputValue: 'Xuhao',
                                margin: '2 35 0 0',
                                labelWidth: 80,
                                listeners: {
                                    change: function (radio, newValue, oldValue) {
                                        var txtXH = Ext.getCmp("Xuhao");
                                        if (newValue) {
                                            txtXH.allowBlank = false;
                                            txtXH.setDisabled(false);
                                        } else {
                                            txtXH.setValue("0");
                                            txtXH.allowBlank = true;
                                            txtXH.setValue("");
                                            txtXH.isValid();
                                            txtXH.setDisabled(true);
                                        }
                                    }
                                }
                            },
                            {
                                xtype: 'textfield',
                                name: 'Xuhao',
                                id: 'Xuhao',
                                margin: '2 0 0 0',
                                // allowBlank: false,
                                disabled: true
                            }
                            ]
                        },
                         {
                             xtype: 'fieldcontainer',
                             combineErrors: true,
                             fieldLabel: USETIMESTART,
                             labelWidth: 80,
                             layout: 'hbox',
                             defaults: {
                                 hideLabel: true
                             },
                             items: [
                             {
                                 xtype: 'radiofield',
                                 name: 'us_start',
                                 width: 10,
                                 inputValue: "us_startToday",
                                 id: "us_s1",
                                 width: 50,
                                 checked: true,
                                 listeners: {
                                     change: function (radio, newValue, oldValue) {
                                         if (newValue) {
                                             Ext.getCmp("us_startToday").setDisabled(false);
                                             Ext.getCmp("us_startChoice").setDisabled(true);
                                         }
                                     }
                                 }
                             },
                            {

                                xtype: 'displayfield',
                                flex: 1,
                                id: 'us_startToday',
                                value: NOWTIME
                            }

                             ]
                         },
                            {
                                xtype: 'fieldcontainer',
                                combineErrors: true,
                                layout: 'hbox',
                                margin: '0 0 0 85',
                                defaults: {
                                    hideLabel: true
                                },
                                items: [
                           {
                               xtype: 'radiofield',
                               name: 'us_start',
                               width: 50,
                               inputValue: "us_startChoice",
                               id: "us_s2",

                               listeners: {
                                   change: function (radio, newValue, oldValue) {
                                       if (newValue) {
                                           Ext.getCmp("us_startChoice").setDisabled(false);
                                           Ext.getCmp("us_startToday").setDisabled(true);
                                       }
                                   }
                               }
                           },
                            {
                                xtype: "datetimefield",
                                id: 'us_startChoice',
                                name: 'us_startChoice',
                                format: 'Y-m-d H:i:s',
                                allowBlank: false,
                                submitValue: true,
                                editable: false,
                                width: 170,
                                value: Tomorrow(),
                                disabled: true
                            }
                                ]
                            },
                       {
                           xtype: 'fieldcontainer',
                           combineErrors: true,
                           fieldLabel: USETIMESEND,
                           labelWidth: 80,
                           layout: 'hbox',
                           defaults: {
                               hideLabel: true
                           },
                           items: [
                           {
                               xtype: 'radiofield',
                               name: 'us_end',
                               inputValue: "use_endMonth",
                               id: "us_e1",
                               width: 50,
                               checked: true,
                               listeners: {
                                   change: function (radio, newValue, oldValue) {
                                       if (newValue) {
                                           Ext.getCmp("use_endMonth").setDisabled(false);
                                           Ext.getCmp("use_endChoice").setDisabled(true);
                                       }
                                   }
                               }
                           },
                            {
                                xtype: 'combobox', //使用日期（迄）
                                allowBlank: true,
                                editable: false,
                                hidden: false,
                                id: 'use_endMonth',
                                name: 'use_endMonth',
                                store: UseEndStore,
                                displayField: 'value',
                                valueField: 'value',
                                typeAhead: true,
                                value: 0,
                                width: 170,
                                forceSelection: false,
                                emptyText: 'SELECT',
                                queryMode: 'local'
                            }
                           ]
                       },
                        {
                            xtype: 'fieldcontainer',
                            combineErrors: true,
                            margin: '0 0 0 85',
                            layout: 'hbox',
                            defaults: {
                                hideLabel: true
                            },
                            items: [
                           {
                               xtype: 'radiofield',
                               name: 'us_end',
                               inputValue: "use_endChoice",
                               id: "us_e2",
                               margin: '0 1 0 0',
                               width: 50,
                               listeners: {
                                   change: function (radio, newValue, oldValue) {
                                       if (newValue) {
                                           Ext.getCmp("use_endMonth").setDisabled(true);
                                           Ext.getCmp("use_endChoice").setDisabled(false);
                                       }
                                   }
                               }
                           }
                            ,
                          {
                              xtype: "datetimefield",
                              id: 'use_endChoice',
                              name: 'use_endChoice',
                              format: 'Y-m-d H:i:s',
                              width: 170,
                              allowBlank: false,
                              submitValue: true,
                              editable: false,
                              disabled: true,
                              value: Tomorrow()
                          }
                            ]
                        }

                        ]
                    }
                    ],
                    listeners: {
                        activate: function (tab, oldtab) {
                            if (oldtab.id != undefined) {
                                Ext.getCmp("proGift").reset();
                                Ext.getCmp("gift_product_number").reset();
                                Ext.getCmp("us_bs1").reset();
                                Ext.getCmp("us_bstartChoice").reset();
                                Ext.getCmp("us_bs2").reset();
                                Ext.getCmp("us_bstartChoice").reset();
                                Ext.getCmp("deduct_welfare").reset();

                                Ext.getCmp("use_bendMonth").reset();
                                Ext.getCmp("us_be1").reset();
                                Ext.getCmp("use_bendChoice").reset();
                                Ext.getCmp("us_be2").reset();
                            }
                        }
                    }
                },
                     {
                         title: GWJANDDYQ,
                         id: 'bonusGift',
                         frame: true,
                         items: [
                             {

                                 xtype: "numberfield",
                                 allowBlank: false,
                                 id: 'deduct_welfare',
                                 name: 'deduct_welfare',
                                 minValue: 0,
                                 value: 0,
                                 fieldLabel: BONUSMONEY
                             },
                               {
                                   xtype: 'radiogroup',
                                   hidden: false,
                                   id: 'giftBonusType',
                                   name: 'giftBonusType',
                                   fieldLabel: BOUNSTYPE,
                                   colName: 'giftBonusType',
                                   width: 400,
                                   defaults: {
                                       name: 'giftBonusType',
                                       margin: '0 8 0 0'
                                   },
                                   columns: 2,
                                   vertical: true,
                                   items: [
                                        { boxLabel: BONUS_ONE, id: 'BonusOne', inputValue: '0', checked: true },
                                        { boxLabel: BONUS_TWO, id: 'BonusTwo', inputValue: '1' }
                                   ]
                               },
                                {
                                    xtype: 'fieldset',
                                    flex: 1,
                                    layout: 'anchor',
                                    items: [
                        {
                            xtype: 'fieldcontainer',
                            combineErrors: true,
                            layout: 'hbox',
                            defaults: {
                                hideLabel: true
                            },
                            items: [
                            {
                                xtype: 'checkbox',
                                boxLabel: CREATEXUHAO,
                                margin: '2 35 0 0',
                                labelWidth: 80,
                                id: 'isXH_GD',
                                name: 'isXH_GD',
                                inputValue: 'inXH_GD',
                                listeners: {
                                    change: function (radio, newValue, oldValue) {
                                        var txtXH = Ext.getCmp("inXH_GD");
                                        if (newValue) {
                                            txtXH.allowBlank = false;
                                            txtXH.setDisabled(false);
                                        } else {
                                            txtXH.setValue("0");
                                            txtXH.allowBlank = true;
                                            txtXH.setValue("");
                                            txtXH.isValid();
                                            txtXH.setDisabled(true);
                                        }
                                    }
                                }
                            },
                            {
                                xtype: 'textfield',
                                name: 'inXH_GD',
                                id: 'inXH_GD',
                                margin: '2 0 0 0',
                                allowBlank: false,
                                disabled: true
                            }
                            ]
                        },
                        {
                            xtype: 'fieldcontainer',
                            combineErrors: true,
                            fieldLabel: USETIMESTART,
                            labelWidth: 80,
                            layout: 'hbox',
                            defaults: {
                                hideLabel: true
                            },
                            items: [
                             {
                                 xtype: 'radiofield',
                                 name: 'us_bstart',
                                 width: 10,
                                 inputValue: "us_bstartToday",
                                 id: "us_bs1",
                                 width: 50,
                                 checked: true,
                                 listeners: {
                                     change: function (radio, newValue, oldValue) {
                                         if (newValue) {
                                             Ext.getCmp("us_bstartToday").setDisabled(false);
                                             Ext.getCmp("us_bstartChoice").setDisabled(true);
                                         }
                                     }
                                 }
                             },
                            {
                                xtype: 'displayfield',
                                flex: 1,
                                id: 'us_bstartToday',
                                value: NOWTIME
                            }
                            ]
                        },
                            {
                                xtype: 'fieldcontainer',
                                combineErrors: true,
                                layout: 'hbox',
                                margin: '0 0 0 85',
                                defaults: {
                                    hideLabel: true
                                },
                                items: [
                           {
                               xtype: 'radiofield',
                               name: 'us_bstart',
                               width: 50,
                               inputValue: "us_bstartChoice",
                               id: "us_bs2",

                               listeners: {
                                   change: function (radio, newValue, oldValue) {
                                       if (newValue) {
                                           Ext.getCmp("us_bstartChoice").setDisabled(false);
                                           Ext.getCmp("us_bstartToday").setDisabled(true);
                                       }
                                   }
                               }
                           },
                            {
                                xtype: "datetimefield",
                                id: 'us_bstartChoice',
                                width: 170,
                                editable: false,
                                name: 'us_bstartChoice',
                                format: 'Y-m-d H:i:s',
                                allowBlank: false,

                                submitValue: true,
                                value: Tomorrow(),
                                disabled: true
                            }
                                ]
                            },
                       {
                           xtype: 'fieldcontainer',
                           combineErrors: true,
                           fieldLabel: USETIMESEND,
                           labelWidth: 80,
                           layout: 'hbox',
                           defaults: {
                               hideLabel: true
                           },
                           items: [
                           {
                               xtype: 'radiofield',
                               name: 'us_bend',
                               inputValue: "use_bendMonth",
                               id: "us_be1",
                               width: 50,
                               checked: true,
                               listeners: {
                                   change: function (radio, newValue, oldValue) {
                                       if (newValue) {
                                           Ext.getCmp("use_bendMonth").setDisabled(false);
                                           Ext.getCmp("use_bendChoice").setDisabled(true);
                                       }
                                   }
                               }
                           },
                            {
                                xtype: 'combobox', //使用日期（迄）
                                allowBlank: true,
                                editable: false,
                                width: 170,
                                hidden: false,
                                id: 'use_bendMonth',
                                name: 'use_bendMonth',
                                store: UseEndStore,
                                displayField: 'value',
                                value: 0,
                                valueField: 'value',
                                typeAhead: true,
                                forceSelection: false,
                                emptyText: 'SELECT',
                                queryMode: 'local'
                            }
                           ]
                       },
                        {
                            xtype: 'fieldcontainer',
                            combineErrors: true,
                            margin: '0 0 0 85',
                            layout: 'hbox',
                            defaults: {
                                hideLabel: true
                            },
                            items: [
                           {
                               xtype: 'radiofield',
                               name: 'us_bend',
                               inputValue: "use_bendChoice",
                               id: "us_be2",
                               margin: '0 1 0 0',
                               width: 50,
                               listeners: {
                                   change: function (radio, newValue, oldValue) {
                                       if (newValue) {
                                           Ext.getCmp("use_bendMonth").setDisabled(true);
                                           Ext.getCmp("use_bendChoice").setDisabled(false);
                                       }
                                   }
                               }
                           }
                            ,
                          {
                              xtype: "datetimefield",
                              id: 'use_bendChoice',
                              name: 'use_bendChoice',
                              width: 170,
                              format: 'Y-m-d H:i:s',
                              allowBlank: false,
                              editable: false,
                              submitValue: true,
                              value: Tomorrow(),
                              disabled: true
                          }
                            ]
                        }]
                                }
                         ],
                         listeners: {
                             activate: function (tab, oldtab) {
                                 if (oldtab.id != undefined) {
                                     Ext.getCmp("proGift").reset();
                                     Ext.getCmp("gift_product_number").reset();
                                     Ext.getCmp("us_s1").reset();
                                     Ext.getCmp("us_startChoice").reset();
                                     Ext.getCmp("us_s2").reset();
                                     Ext.getCmp("us_startChoice").reset();
                                     Ext.getCmp("use_endMonth").reset();
                                     Ext.getCmp("us_e1").reset();
                                     Ext.getCmp("use_endChoice").reset();
                                     Ext.getCmp("us_e2").reset();


                                 }
                             }
                         }
                     },
                ]
            });

    var firstForm = Ext.widget('form',
           {
               id: 'editFrm1',
               plain: true,
               frame: true,
               defaultType: 'textfield',
               layout: 'anchor',
               defaults: { anchor: "95%", msgTarget: "side" },
               items: [
                {
                    fieldLabel: ACTIVENAME,
                    xtype: 'textfield',
                    padding: '10 0 5 0',
                    id: 'name',
                    name: 'name',
                    allowBlank: false,
                    labelWidth: 80
                },
                {
                    fieldLabel: ACTIVEDESC,
                    xtype: 'textfield',
                    padding: '10 0 5 0',
                    id: 'event_desc',
                    name: 'event_desc',
                    allowBlank: false,
                    labelWidth: 80
                },
                {
                    xtype: "numberfield",
                    fieldLabel: VENDORCOVERAGE,
                    id: 'vendor_coverage',
                    name: 'vendor_coverage',
                    allowBlank: false,
                    padding: '10 0 5 0',
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
                         anchor: '100%'
                     },
                     items: [
                 {
                     xtype: 'fieldcontainer',
                     fieldLabel: ALLMONEY,
                     combineErrors: true,
                     layout: 'hbox',
                     defaults: {
                         flex: 1,
                         hideLabel: true
                     },
                     items: [
                     {
                         xtype: 'radiofield',
                         name: 'ra',
                         inputValue: "amount",
                         id: "ra1",
                         checked: true,
                         listeners: {
                             change: function (radio, newValue, oldValue) {
                                 var amountCP = Ext.getCmp("amount");
                                 var quantityCP = Ext.getCmp("quantity");
                                 if (newValue) {
                                     quantityCP.setValue("0");
                                     quantityCP.allowBlank = true;
                                     quantityCP.setValue("");
                                     quantityCP.setDisabled(true);

                                     amountCP.setDisabled(false);
                                     amountCP.allowBlank = false;

                                 }
                             }
                         }

                     },
                     {
                         xtype: "numberfield",
                         id: 'amount',
                         name: 'amount',
                         allowBlank: false,
                         minValue: 0
                     }
                     ]
                 }, {
                     xtype: 'fieldcontainer',
                     fieldLabel: FAREQUANTITY,
                     combineErrors: true,
                     layout: 'hbox',
                     margins: '0 100 0 0',
                     defaults: {
                         flex: 1,
                         width: 120,
                         hideLabel: true
                     },
                     items: [
                     {
                         xtype: 'radiofield',
                         name: 'ra',
                         inputValue: "quantity",
                         id: "ra2",
                         listeners: {
                             change: function (radio, newValue, oldValue) {
                                 var amountCP = Ext.getCmp("amount");
                                 var quantityCP = Ext.getCmp("quantity");
                                 if (newValue) {
                                     amountCP.setValue("0");
                                     amountCP.allowBlank = true;
                                     amountCP.setValue("");

                                     amountCP.setDisabled(true);
                                     quantityCP.setDisabled(false);
                                     quantityCP.allowBlank = false;
                                 }
                             }
                         }
                     },
                     {
                         xtype: "numberfield",
                         id: 'quantity',
                         name: 'quantity',
                         allowBlank: false,
                         disabled: true,
                         minValue: 0
                     }
                     ]

                 }
                     ]
                 }
               ]
           });
    //第二個form
    var secondForm = Ext.widget('form',
        {
            id: 'editFrm2',
            frame: true,
            plain: true,
            defaultType: 'textfield',
            layout: 'anchor',
            autoScroll: true,
            height: 400,
            defaults: { anchor: "95%", msgTarget: "side" },
            items: [
             {
                 xtype: 'fieldset',
                 defaults: {
                     labelWidth: 89,
                     anchor: '95%',
                     layout: {
                         type: 'hbox'
                     }
                 },
                 items: [
                        {
                            //會員群組和會員條件二擇一
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
                                    width: 90,
                                    checked: true,
                                    listeners: {
                                        change: function (radio, newValue, oldValue) {
                                            var rdo_group = Ext.getCmp("us1");
                                            var rdo_groppset = Ext.getCmp("us2");
                                            var com_group = Ext.getCmp("group_id");
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
                                    id: 'group_id',
                                    name: 'group_name',
                                    hiddenName: 'group_id',
                                    store: VipGroupStore,
                                    displayField: 'group_name',
                                    valueField: 'group_id',
                                    typeAhead: true,
                                    lastQuery: '',
                                    forceSelection: false,
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
                                    width: 90,
                                    inputValue: "u_groupset",
                                    id: "us2",
                                    listeners: {
                                        change: function (radio, newValue, oldValue) {
                                            var rdo_group = Ext.getCmp("us1");
                                            var rdo_groppset = Ext.getCmp("us2");
                                            var com_group = Ext.getCmp("group_id");
                                            var btn_group = Ext.getCmp("condi_set");
                                            if (newValue) {
                                                com_group.setValue("0");
                                                com_group.allowBlank = true;
                                                com_group.setValue("");
                                                com_group.isValid();
                                                btn_group.setDisabled(false);
                                                com_group.setDisabled(true);
                                                if (condition_id != "") {
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
                                    allowBlank: false,
                                    id: 'condi_set',
                                    colName: 'condi_set',
                                    handler: function () {
                                        if (conditionID != "") {
                                            showUserSetForm(condition_name, conditionID, "userInfo");
                                        } else {
                                            showUserSetForm(condition_name, condition_id, "userInfo");
                                        }
                                    }
                                }
                            ]
                        },
                        {
                            xtype: 'fieldset',
                            title: USERDETAIL,
                            defaultType: 'textfield',
                            id: 'userShow',
                            layout: 'anchor',
                            width: '100%',
                            hidden: true,
                            items: [
                                      {
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
                        }
                 ]
             },

                {
                    xtype: 'fieldset',
                    defaults: {
                        anchor: '95%',
                        layout: {
                            type: 'hbox',
                            defaultMargins: { top: 50, bottom: 0 }
                        }
                    },
                    items: [
                        {
                            xtype: 'radiogroup',
                            hidden: false,
                            id: 'count_by',
                            name: 'count_by',
                            fieldLabel: NUMBERLIMIT,
                            colName: 'count_by',
                            width: 400,
                            defaults: {
                                name: 'count_by',
                                margin: '0 8 0 0'
                            },
                            columns: 3,
                            vertical: true,
                            items: [
                                {
                                    boxLabel: NOTHING,
                                    name: 'count_by',
                                    id: 'rdoNoTimesLimit',
                                    inputValue: "1",
                                    checked: true,
                                    listeners: {
                                        change: function (radio, newValue, oldValue) {
                                            var numLimit = Ext.getCmp("num_limit");
                                            if (newValue) {
                                                numLimit.setValue(0);
                                                numLimit.setDisabled(true);
                                            }
                                        }
                                    }
                                },
                                {
                                    boxLabel: BYORDER,
                                    name: 'count_by',
                                    id: 'rdoByOrder',
                                    inputValue: "2",
                                    listeners: {
                                        change: function (radio, newValue, oldValue) {
                                            var numLimit = Ext.getCmp("num_limit");
                                            if (newValue) {
                                                numLimit.setValue(0);
                                                numLimit.setDisabled(false);
                                            }
                                        }
                                    }
                                },
                                 {
                                     boxLabel: BYMEMBER,
                                     name: 'count_by',
                                     id: 'rdoByMember',
                                     inputValue: "3",
                                     listeners: {
                                         change: function (radio, newValue, oldValue) {
                                             var numLimit = Ext.getCmp("num_limit");
                                             if (newValue) {
                                                 numLimit.setValue(0);
                                                 numLimit.setDisabled(false);
                                             }
                                         }
                                     }
                                 }
                            ]
                        },
                        {
                            xtype: 'numberfield',
                            fieldLabel: PRESENTTIMES,
                            name: 'num_limit',
                            id: 'num_limit',
                            minValue: 0,
                            value: 0,
                            disabled: true
                        },
                        {
                            xtype: 'radiogroup',
                            hidden: false,
                            id: 'repeat',
                            name: 'repeat',
                            fieldLabel: ISREPEATGIFT,
                            colName: 'repeat',
                            defaults: {
                                name: 'IsRepeat',
                                margin: '0 8 0 0'
                            },
                            columns: 2,
                            vertical: true,
                            items: [
                                { boxLabel: ONLYONE, id: 'OnlyOne', inputValue: '0', checked: true },
                                { boxLabel: EVERY, id: 'Every', inputValue: '1' }
                            ]
                        }
                    ]
                },
                {
                    xtype: 'combobox',
                    fieldLabel: YSCLASS,
                    store: YunsongStore,
                    queryMode: 'local',
                    allowBlank: false,
                    editable: false,
                    id: 'freight',
                    name: 'freight',
                    hiddenName: 'typeName',
                    colName: 'typeName',
                    displayField: 'type_name',
                    valueField: 'type_id',
                    typeAhead: true,
                    forceSelection: false,
                    value: '0',
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
                    xtype: 'combobox',
                    fieldLabel: PAYTYPE,
                    id: 'payment',
                    multiSelect: true, //支持多選
                    queryMode: 'local',
                    name: 'payment',
                    allowBlank: false,
                    editable: false,
                    typeAhead: true,
                    forceSelection: false,
                    store: paymentStore,
                    displayField: 'parameterName',
                    valueField: 'parameterCode',
                    value: '0',
                    emptyText: SELECT
                },
                {
                    xtype: "datetimefield",
                    fieldLabel: BEGINTIME,
                    editable: false,
                    id: 'startdate',
                    name: 'startdate',
                    anchor: '95%',
                    format: 'Y-m-d H:i:s',
                    width: 150,
                    allowBlank: false,
                    submitValue: true,
                    value: Tomorrow(),
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("startdate");
                            var end = Ext.getCmp("enddate");
                            var s_date = new Date(start.getValue());
                            var ttime = s_date;
                            ttime = new Date(ttime.setMonth(s_date.getMonth() + 1));
                            ttime = new Date(ttime.setMinutes(59));
                            ttime = new Date(ttime.setSeconds(59));
                            ttime = new Date(ttime.setHours(23));
                            end.setValue(ttime);
                        }
                    }
                },
                {
                    xtype: "datetimefield",
                    fieldLabel: ENDTIME,
                    editable: false,
                    id: 'enddate',
                    anchor: '95%',
                    name: 'enddate',
                    format: 'Y-m-d H:i:s',
                    allowBlank: false,
                    submitValue: true,
                    value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() + 1)),
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("startdate");
                            var end = Ext.getCmp("enddate");
                            var s_date = new Date(start.getValue());
                            var now_date = new Date(end.getValue());
                            if (end.getValue() < start.getValue()) {
                                Ext.Msg.alert(INFORMATION, TIMETIP);
                            }
                            var ttime = now_date;
                            ttime = new Date(ttime.setMonth(now_date.getMonth()));
                            ttime = new Date(ttime.setMinutes(59));
                            ttime = new Date(ttime.setSeconds(59));
                            ttime = new Date(ttime.setHours(23));
                            end.setValue(ttime);
                        }
                    }
                },
                {
                    xtype: 'combobox', //Wibset
                    fieldLabel: WEBSITESET,
                    id: 'site',
                    name: 'site',
                    hiddenName: 'site',
                    colName: 'site',
                    allowBlank: false,
                    editable: false,
                    store: SiteStore,
                    displayField: 'Site_Name',
                    valueField: 'Site_Id',
                    typeAhead: true,
                    forceSelection: false,
                    emptyText: SELECT,
                    multiSelect: true, //支持多選
                    queryMode: 'local',
                    listeners: {
                        select: function (a, b, c) {
                            websiteID = Ext.htmlEncode(Ext.getCmp('site').getValue());
                        }
                    }
                }
            ]
        });
    var thirdForm = Ext.widget('form',
           {
               id: 'editFrm3',
               frame: true,
               plain: true,
               url: ' /PromotionsAmountGift/SecondSaveGift',
               defaultType: 'textfield',
               layout: 'anchor',
               buttons: [
               {
                   text: SAVE,
                   formBind: true,
                   disabled: true,
                   id: 'pSave',
                   handler: function () {
                    
                       var form = this.up('form').getForm();

                       if (Ext.getCmp("url_by").getValue().url_by == "1" && !Ext.getCmp('allClass').getValue() && (condiCount == 0 && RowID == null) && (Ext.getCmp('brand_id').getValue() == "" || Ext.getCmp('brand_id').getValue() == null || Ext.getCmp('brand_id').getValue() == "0")) {
                           Ext.Msg.alert(INFORMATION, CONDITIONTIP);
                           return;
                       }
                       if (RowID) {
                           if (RowID.data.product_id == 999999)//編輯時當原數據是全館時判斷是否點擊條件設定或者設置品牌
                           {
                               if (Ext.getCmp("url_by").getValue().url_by == "1" && condiCount == 0 && (Ext.getCmp('brand_id').getValue() == "" || Ext.getCmp('brand_id').getValue() == null || Ext.getCmp('brand_id').getValue() == "0") && !Ext.getCmp('allClass').getValue()) {
                                   Ext.Msg.alert(INFORMATION, CONDITIONTIP);
                                   return;
                               }
                           }
                       }

                       if (condition_id == "" && Ext.getCmp("group_id").getValue() == null) {
                           Ext.Msg.alert(INFORMATION, USERCONDITIONERROR);
                           return;
                       }
                       var Xuhao = "";
                       var activeNow = 0;
                       var startdate = "";
                       var enddate = "";
                       var gift_type = Ext.htmlEncode(activeTypetabPanel.getActiveTab().title);
                       var boolValid = false;

                       switch (gift_type) {
                           case PRODUCT:
                               Ext.getCmp('proGift').allowBlank = false;
                               Ext.getCmp('gift_product_number').allowBlank = false;
                               if (Ext.getCmp("proGift").isValid()) {
                                   gift_type = 1;
                                   boolValid = true;
                               } else {
                                   return;
                               }
                               break;
                           case CHANCE:

                               Ext.getCmp('proGift').allowBlank = true;
                               Ext.getCmp('gift_product_number').allowBlank = true;
                               if (Ext.getCmp("Xuhao").isValid()) {
                                   Xuhao = Ext.getCmp("Xuhao").getValue();
                                   if (Ext.getCmp("us_s1").getValue()) {
                                       activeNow = 1;
                                       startdate = new Date();
                                   } else {
                                       activeNow = 0;
                                       startdate = Ext.getCmp("us_startChoice").getValue();
                                   }
                                   enddate = new Date(startdate);
                                   if (Ext.getCmp("us_e1").getValue()) {
                                       var u_endMoneth = Ext.getCmp("use_endMonth");
                                       var month = enddate.getMonth() + u_endMoneth.getValue();
                                       enddate = formatDate(new Date(enddate.setMonth(month)));
                                   } else {
                                       enddate = formatDate(Ext.getCmp("use_endChoice").getValue());
                                   }
                                   startdate = formatDate(startdate);
                                   gift_type = 2;

                                   boolValid = true;
                               }
                               break;
                           case GWJANDDYQ:
                               Ext.getCmp('proGift').allowBlank = true;
                               Ext.getCmp('gift_product_number').allowBlank = true;
                               if (Ext.getCmp("deduct_welfare").isValid() && Ext.getCmp("inXH_GD").isValid()) {
                                   Xuhao = Ext.getCmp("inXH_GD").getValue();
                                   if (Ext.getCmp("us_bs1").getValue()) {
                                       activeNow = 1;
                                       startdate = new Date();
                                   } else {
                                       activeNow = 0;
                                       startdate = Ext.getCmp("us_bstartChoice").getValue();
                                   }
                                   enddate = new Date(startdate);
                                   if (Ext.getCmp("us_be1").getValue()) {
                                       var u_bendMoneth = Ext.getCmp("use_bendMonth");
                                       var bmonth = enddate.getMonth() + u_bendMoneth.getValue();

                                       enddate = formatDate(new Date(enddate.setMonth(bmonth)));
                                   } else {
                                       enddate = formatDate(Ext.getCmp("use_bendChoice").getValue());
                                   }
                                   startdate = formatDate(startdate);
                                   gift_type = 2;
                                   if (Ext.getCmp("giftBonusType").getValue().giftBonusType == 0) {
                                       gift_type = 3;
                                   } else {
                                       gift_type = 4;
                                   }

                                   boolValid = true;
                               }
                               break;
                       }



                       if (boolValid) {
                           this.disable();
                           form.submit({
                               params: {
                                   isEdit: RowID,
                                   rowid: promoID,
                                   cateid: prodCateID,
                                   event_name: Ext.getCmp("name").getValue(),
                                   event_desc: Ext.getCmp("event_desc").getValue(),
                                   event_type: condition_name,
                                   url_by: Ext.htmlEncode(Ext.getCmp("url_by").getValue().url_by),
                                   banner_url: Ext.htmlEncode(Ext.getCmp("banner_url").getValue()),
                                   group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                                   condition_id: condition_id,
                                   amount: Ext.htmlEncode(Ext.getCmp("amount").getValue()),
                                   quantity: Ext.htmlEncode(Ext.getCmp("quantity").getValue()),
                                   sclass_id: Ext.htmlEncode(Ext.getCmp('class_id').getValue()),
                                   sbrand_id: document.getElementsByName('brand_name')[0].value,
                                   allClass: Ext.htmlEncode(Ext.getCmp("allClass").getValue()),
                                   count_by: Ext.htmlEncode(Ext.getCmp('count_by').getValue().count_by),
                                   numLimit: Ext.htmlEncode(Ext.getCmp('num_limit').getValue()),
                                   IsRepeat: Ext.htmlEncode(Ext.getCmp('repeat').getValue().IsRepeat),
                                   frieghttype: Ext.htmlEncode(Ext.getCmp('freight').getValue()),
                                   devicename: Ext.htmlEncode(Ext.getCmp("deviceName").getValue().deviceName),
                                   payment: Ext.htmlEncode(Ext.getCmp('payment').getValue()),
                                   start_time: Ext.htmlEncode(Ext.getCmp('startdate').getRawValue()),
                                   end_time: Ext.htmlEncode(Ext.getCmp('enddate').getRawValue()),
                                   site: Ext.htmlEncode(Ext.getCmp('site').getValue()),
                                   gift_type: Ext.htmlEncode(gift_type),
                                   gift_id: Ext.htmlEncode(Ext.getCmp('proGift').getValue()),
                                   chanceXuhao: Ext.htmlEncode(Ext.getCmp('chanceXuhao').getValue()).ChanceIsXuhao,
                                   IsRepeatXuhao: Ext.htmlEncode(Ext.getCmp('IsRepeatXuhao').getValue()).rIsRepeatXuhao,
                                   Xuhao: Ext.htmlEncode(Xuhao),
                                   activeNow: Ext.htmlEncode(activeNow),
                                   startdate: Ext.htmlEncode(startdate),
                                   enddate: Ext.htmlEncode(enddate),
                                   bonusAmount: Ext.htmlEncode(Ext.getCmp('deduct_welfare').getValue()),
                                   vendor_coverage: Ext.htmlEncode(Ext.getCmp('vendor_coverage').getValue()),
                                   gift_product_number: Ext.htmlEncode(Ext.getCmp('gift_product_number').getValue()),
                                   bonusType: Ext.htmlEncode(Ext.getCmp('giftBonusType').getValue()).giftBonusType,
                                   noclass_id: Ext.htmlEncode(Ext.getCmp('no_class_id').getValue()),
                                   nobrand_id: Ext.htmlEncode(Ext.getCmp('no_brand_id').getValue()),
                                   product_id: Ext.htmlEncode(Ext.getCmp('product_id').getValue()),
                                   noallClass: Ext.htmlEncode(Ext.getCmp("allClass_no").getValue())
                               },
                               success: function (form, action) {
                                   var result = Ext.decode(action.response.responseText);
                                   if (result.success) {
                                       Ext.Msg.alert(INFORMATION, SUCCESS);
                                       PromoAmountGiftStore.load();
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
               }],
               defaults: { anchor: "95%", msgTarget: "side" },
               items: [
                {

                    xtype: 'radiogroup',
                    fieldLabel: ISBANNERURL,
                    id: 'url_by',
                    colName: 'url_by',
                    name: 'url_by',
                    defaults: {
                        name: 'url_by'
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
                                           var isBannerPanel = Ext.getCmp("isBanner");
                                           var noBannerPanel = Ext.getCmp("noBanner");
                                           if (newValue) {
                                               noBannerPanel.hide();
                                               var banner_image = Ext.getCmp("banner_url");
                                               banner_image.setValue(linkPath + GetEventId(condition_name, promoID));
                                               banner_url.allowBlank = false;
                                               photo.allowBlank = false;
                                               isBannerPanel.show();

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
                                           var allClass = Ext.getCmp("allClass");
                                           var photo = Ext.getCmp("photo");
                                           var isBannerPanel = Ext.getCmp("isBanner");
                                           var noBannerPanel = Ext.getCmp("noBanner");
                                           if (newValue) {
                                               isBannerPanel.hide();
                                               banner_url.setValue("0");
                                               banner_url.allowBlank = true;
                                               banner_url.setValue("");
                                               banner_url.isValid();
                                               //   banner_url.hide();
                                               photo.setValue("0");
                                               photo.allowBlank = true;
                                               photo.setValue("");
                                               photo.isValid();
                                               // photo.hide();
                                               noBannerPanel.show();
                                               allClass.setDisabled(false);
                                           }
                                       }
                                   }
                               }
                    ]
                },
                 {
                     xtype: 'form', //是專區顯示
                     frame: true,
                     plain: true,
                     hidden: true,
                     layout: 'anchor',
                     border: 0,
                     id: 'isBanner',
                     style: 'border-width:0 0 0 0 ;',//去邊框
                     defaults: { anchor: "95%", msgTarget: "side" },
                     items: [
               {
                   xtype: 'textfield',
                   fieldLabel: SELFURL,
                   name: 'category_link_url',
                   id: 'banner_url',
                   labelWidth: 80,
                   submitValue: true,
                   vtype: 'url',
                   allowBlank: true
               },
               {//專區Banner
                   xtype: 'filefield',
                   name: 'photo',
                   id: 'photo',
                   fieldLabel: BANNERIMG,
                   labelWidth: 80,
                   msgTarget: 'side',
                   anchor: '95%',
                   buttonText: SELECT_IMG,
                   submitValue: true,
                   allowBlank: true,
                   fileUpload: true
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
                            var sclass = Ext.getCmp('class_id');
                            var brand = Ext.getCmp('brand_id');
                            var allClass = Ext.getCmp('allClass');
                            var btn_condi = Ext.getCmp('p_condi');
                            brand.clearValue();
                            if (sclass.getValue() != null) {
                                brand.setDisabled(false);
                            } else if (sclass.getValue() == null) {
                                brand.setValue("");
                                brand.setDisabled(true);
                            }
                            classBrandStore.removeAll();
                            boolClass = true;

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
                     //  hidden: true,
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
                  {
                      //促銷商品類別
                      xtype: 'fieldcontainer', //類別設定
                      hidden: false,
                      layout: 'hbox',
                      id: 'cx',
                      border: 0,
                      id: 'p_sortpanl',
                      colName: 'user_codi',
                      items: [
                        {
                            xtype: 'label',
                            text: PROSORT1,
                            id: 'p_sort',
                            margin: '0 8 0 0'
                        },
                        {
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
                  }
                     ]
                 }
                  ,
                     {
                         xtype: 'form', //非專區顯示
                         frame: true,
                         plain: true,
                         layout: 'anchor',
                         border: 0,
                         id: 'noBanner',
                         style: 'border-width:0 0 0 0 ;',//去邊框
                         defaults: { anchor: "95%", msgTarget: "side" },
                         items: [

                    {
                        xtype: 'combobox', //class_id
                        fieldLabel: SHOPCLASS,
                        editable: false,
                        id: 'no_class_id',
                        name: 'class_id',
                        hiddenName: 'class_id',
                        colName: 'class_id',
                        allowBlank: true,
                        store: ShopClassStore,
                        displayField: 'class_name',
                        valueField: 'class_id',
                        typeAhead: true,
                        forceSelection: false,
                        emptyText: SELECT,
                        queryMode: 'remote',
                        lastQuery: '',
                        value: 0,
                        listeners: {
                            "select": function (combo, record) {
                                var z = Ext.getCmp("no_brand_id");
                                var p = Ext.getCmp("product_id");
                                var n = Ext.getCmp("product_name");

                                z.setValue(0);
                                p.setValue("");//當館別改變時清空商品編號
                                n.setValue("");
                                classBrandStore_no.removeAll();
                                boolClass = true;
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
                       id: 'no_brand_id',
                       name: 'brand_name',
                       hiddenname: 'brand_id',
                       store: classBrandStore_no,
                       displayField: 'brand_name',
                       valueField: 'brand_id',
                       typeAhead: true,
                       forceSelection: false,
                       emptyText: SELECT,
                       value: 0,
                       listeners: {
                           beforequery: function (qe) {
                               delete qe.combo.lastQuery;
                               classBrandStore_no.load({
                                   params: {
                                       topValue: Ext.htmlEncode(Ext.getCmp("no_class_id").getValue())
                                   },
                                   callback: function () {
                                       if (!RowID) {
                                           Ext.getCmp("no_brand_id").setValue(classBrandStore_no.data.items[0].data.brand_id);
                                       }
                                       else {

                                           Ext.getCmp("no_brand_id").setValue(RowID.data.brand_id);
                                       }
                                   }
                               });
                           },
                           'blur': function () {
                               var o = classBrandStore_no.data.items;
                               var beand_name_frank = '';
                               Ext.getCmp("product_id").setValue("");//當品牌改變時清空商品編號
                               Ext.getCmp("product_name").setValue("");//當品牌改變時清空商品編號
                               if (classBrandStore_no.getCount() > 0) {
                                   if (document.getElementsByName('brand_name')[0].value != Ext.getCmp('no_brand_id').getValue()) {
                                       document.getElementsByName('brand_name')[0].value = classBrandStore_no.getAt(0).get('no_brand_id');
                                   }
                               }
                           }
                       }
                   }
                                ,
                                 {
                                     //促銷商品類別
                                     xtype: 'fieldcontainer', //非專區商品
                                     hidden: false,
                                     layout: 'hbox',
                                     id: 'cx',
                                     border: 0,
                                     id: 'p_sortpanl_no',
                                     colName: 'user_codi',
                                     items: [
                                  {
                                      xtype: "numberfield",
                                      fieldLabel: PRODID,
                                      id: 'product_id',
                                      name: 'product_id',
                                      allowBlank: true,
                                      minValue: 0,
                                      submitValue: true,
                                      listeners: {
                                          'blur': function () {
                                              var pid = Ext.getCmp("product_id");
                                              var pname = Ext.getCmp("product_name");
                                              var cid = Ext.getCmp("no_class_id");
                                              var bid = Ext.getCmp("no_brand_id");

                                              if (pid.getValue() != "" && pid.getValue() != null) {
                                                  Ext.Ajax.request({
                                                      url: '/PromotionsDiscount/GetProdName',
                                                      params: {
                                                          product_id: Ext.htmlEncode(pid.getValue()),
                                                          class_id: Ext.htmlEncode(cid.getValue()),
                                                          brand_id: Ext.htmlEncode(bid.getValue())
                                                      },
                                                      success: function (form, action) {
                                                          var result = Ext.decode(form.responseText);
                                                          if (result.success) {
                                                              pname.setValue(result.prod_name);
                                                          } else {
                                                              Ext.Msg.alert(INFORMATION, PRODTIP);
                                                              pid.setValue(0);
                                                          }
                                                      },
                                                      failure: function () {
                                                          Ext.Msg.alert(INFORMATION, PRODTIP);
                                                          pid.setValue(0);
                                                      }
                                                  });
                                              }
                                              else {
                                                  pname.setValue("");
                                              }
                                              Ext.getCmp('allClass_no').setValue(false);
                                          }
                                      }
                                  },
                                   {
                                       xtype: 'checkbox',
                                       boxLabel: ALLSHOP,
                                       id: 'allClass_no',
                                       name: 'allClass',
                                       margin: '0 0 0 20',
                                       inputValue: '1',
                                       listeners: {
                                           change: function (checkbox, newValue, oldValue) {
                                               var sclass_no = Ext.getCmp('no_class_id');
                                               var brand_no = Ext.getCmp('no_brand_id');
                                               var prod_no = Ext.getCmp('product_id');
                                               if (newValue) {
                                                   sclass_no.setValue(0);
                                                   brand_no.setValue(0);
                                                   prod_no.setValue(999999);
                                                   Ext.getCmp('product_name').setValue("全館商品");
                                               }
                                               else {
                                                   prod_no.setValue(0);

                                                   Ext.getCmp('product_name').setValue("");
                                               }

                                           }
                                       }
                                   }]
                                 },
                                  {
                                      xtype: 'displayfield',
                                      id: 'product_name',
                                      fieldLabel: PRODNAME
                                  }

                         ]
                     },

               {
                   xtype: 'fieldset',
                   title: GIFTTYPE,
                   defaultType: 'textfield',
                   layout: 'anchor',
                   defaults: {
                       anchor: '100%'
                   },
                   items: [activeTypetabPanel]
               }
               ]
           });
    var allpan = new Ext.panel.Panel({
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
                margins: '0 5 0 0',
                handler: function (btn) {
                    navigate(btn.up("panel"), "next");
                }
            }
        ],
        items: [firstForm, secondForm, thirdForm]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: GIFTTITLE,
        //        id: 'editWin2',
        iconCls: RowID ? "icon-user-edit" : "icon-user-add",
        width: 410,
        y: 100,
        //height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [allpan],
        constrain: true, //束縛
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
                    secondForm.getForm().loadRecord(RowID);
                    thirdForm.getForm().loadRecord(RowID);
                    initForm(RowID);
                }
                else {
                    classBrandStore_no.load({
                        callback: function () {
                            Ext.getCmp("no_brand_id").setValue(0);
                        }
                    });
                    firstForm.getForm().reset();
                    secondForm.getForm().reset();
                    thirdForm.getForm().reset();
                }

            }
        }
    });
    editWin.show();
    function formatDate(now) {
        var year = now.getFullYear();
        var month = now.getMonth() + 1;
        var date = now.getDate();
        var hour = now.getHours();
        var minute = now.getMinutes();
        var second = now.getSeconds();
        return year + "-" + month + "-" + date + "   " + hour + ":" + minute + ":" + second;
    };
    function formatDay(now) {
        var year = now.getFullYear();
        var month = now.getMonth() + 1;
        var date = now.getDate();
        return year + "-" + month + "-" + date;
    };
    function initForm(row) {
        var today = formatDay(new Date());

        if (row.data.url_by == 1) {//是專區
            if (row.data.class_id != 0 && row.data.brand_id != 0) {
                Ext.getCmp('brand_id').setDisabled(false);
                classBrandStore.load({
                    params: {
                        topValue: Ext.getCmp("class_id").getValue()
                    }
                });
            }
            if (row.data.class_id == "0") {
                Ext.getCmp('class_id').setValue("");
            }
            if (row.data.product_id == 999999 && row.data.brand_id == 0) {
                Ext.getCmp("allClass").setValue(true);
            }
            Ext.getCmp("bt1").setValue(true);
            var img = row.data.banner_image.toString();
            var imgUrl = img.substring(img.lastIndexOf("\/") + 1);

            Ext.getCmp('photo').setRawValue(imgUrl);
        }
        else {
            Ext.getCmp("bt2").setValue(true);
            Ext.getCmp("banner_url").setValue("");
            //賦值非專區
            Ext.getCmp('no_class_id').setValue(row.data.class_id);
            classBrandStore_no.load({
                params: {
                    topValue: row.data.class_id
                },
                callback: function () {
                    Ext.getCmp("no_brand_id").setValue(row.data.brand_id);
                }
            });

            if (row.data.product_id == 999999) {
                Ext.getCmp("allClass_no").setValue(true);
                Ext.getCmp('product_id').setValue(row.data.product_id);
                Ext.getCmp('product_name').setValue("全館商品");
            }
            if (row.data.product_id != 0 && row.data.product_id != 999999) {

                Ext.Ajax.request({
                    url: '/PromotionsDiscount/GetProdName',
                    params: {
                        product_id: Ext.htmlEncode(row.data.product_id),
                        class_id: Ext.htmlEncode(row.data.class_id),
                        brand_id: Ext.htmlEncode(row.data.brand_id)
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.getCmp('product_name').setValue(result.prod_name);
                        } else {
                            Ext.Msg.alert(INFORMATION, "該商品編號不符合要求！請重新填寫！");
                            Ext.getCmp('product_name').setValue("");
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }

            Ext.getCmp('product_id').setValue(row.data.product_id);


        }


        if (row.data.amount == 0) {
            Ext.getCmp("amount").setValue("")
        } else {
            if (row.data.quantity == 0) {
                Ext.getCmp("quantity").setValue("")
            }
        }
        if (row.data.condition_id != 0) {
            Ext.getCmp('us1').setValue(false);
            Ext.getCmp('us2').setValue(true);
        } else {
            if (row.data.group_id == 0 || row.data.group_id == "") {
                Ext.getCmp('us1').setValue(true);
                Ext.getCmp('us2').setValue(false);
                Ext.getCmp('group_id').setValue(0);
            }
        }

        if (row.data.payment_code == 0) {

            Ext.getCmp("payment").setValue(DEVICE_1);
        }

        switch (row.data.device) {
            case 1:
                Ext.getCmp("pc").setValue(true);
                break;
            case 4:
                Ext.getCmp("mobilepad").setValue(true);
                break;
            default:
                Ext.getCmp("bufen").setValue(true);
                break;
        }

        switch (row.data.event_type) {
            case 'G1':
                Ext.getCmp("ra1").setValue(true);
                Ext.getCmp("quantity").setDisabled(true);
                Ext.getCmp("amount").setDisabled(false);
                Ext.getCmp("amount").setValue(row.data.amount);
                break;
            case 'G2':
                Ext.getCmp("ra2").setValue(true);
                Ext.getCmp("quantity").setValue(row.data.quantity);
                Ext.getCmp("quantity").setDisabled(false);
                Ext.getCmp("amount").setDisabled(true);
                break;
        }
        switch (row.data.gift_type) {
            case 1:
                activeTypetabPanel.setActiveTab("product");
                Ext.getCmp("proGift").setValue(row.data.gift_id);
                break;
            case 2:
                activeTypetabPanel.setActiveTab("chanceGift");
                Ext.getCmp("us_startToday").setDisabled(true);
                Ext.getCmp("us_s1").setValue(false);
                Ext.getCmp("us_startChoice").setDisabled(false);
                Ext.getCmp("us_s2").setValue(true);
                Ext.getCmp("us_startChoice").setValue(row.data.use_start);


                Ext.getCmp("use_endMonth").setDisabled(true);
                Ext.getCmp("us_e1").setValue(false);
                Ext.getCmp("use_endChoice").setDisabled(false);
                Ext.getCmp("us_e2").setValue(true);
                Ext.getCmp("use_endChoice").setValue(row.data.use_end);
                break;
            case 3:
                activeTypetabPanel.setActiveTab("bonusGift");
                Ext.getCmp("BonusOne").setValue(true);
                Ext.getCmp("us_bstartToday").setDisabled(true);
                Ext.getCmp("us_bs1").setValue(false);
                Ext.getCmp("us_bstartChoice").setDisabled(false);
                Ext.getCmp("us_bs2").setValue(true);
                Ext.getCmp("us_bstartChoice").setValue(row.data.use_start);


                Ext.getCmp("use_bendMonth").setDisabled(true);
                Ext.getCmp("us_be1").setValue(false);
                Ext.getCmp("use_bendChoice").setDisabled(false);
                Ext.getCmp("us_be2").setValue(true);
                Ext.getCmp("use_bendChoice").setValue(row.data.use_end);

                break;
            case 4:
                activeTypetabPanel.setActiveTab("bonusGift");
                Ext.getCmp("BonusTwo").setValue(true);
                Ext.getCmp("us_bstartToday").setDisabled(true);
                Ext.getCmp("us_bs1").setValue(false);
                Ext.getCmp("us_bstartChoice").setDisabled(false);
                Ext.getCmp("us_bs2").setValue(true);
                Ext.getCmp("us_bstartChoice").setValue(row.data.use_start);
                Ext.getCmp("use_bendMonth").setDisabled(true);
                Ext.getCmp("us_be1").setValue(false);
                Ext.getCmp("use_bendChoice").setDisabled(false);
                Ext.getCmp("us_be2").setValue(true);
                Ext.getCmp("use_bendChoice").setValue(row.data.use_end);
                break;
        }
        if (row.data.freight == "") {
            Ext.getCmp("freight").setValue(0);
        }


        switch (row.data.repeat) {
            case false:
                Ext.getCmp("OnlyOne").setValue(true);
                break;
            case true:
                Ext.getCmp("Every").setValue(true);
                break;
        }
        switch (row.data.count_by) {
            case 1:
                Ext.getCmp("rdoNoTimesLimit").setValue(true);
                Ext.getCmp("rdoByOrder").setValue(false);
                Ext.getCmp("rdoByMember").setValue(false);
                break;
            case 2:
                Ext.getCmp("rdoNoTimesLimit").setValue(false);
                Ext.getCmp("rdoByOrder").setValue(true);
                Ext.getCmp("rdoByMember").setValue(false);
                Ext.getCmp("num_limit").setValue(row.data.num_limit);
                break;
            case 3:
                Ext.getCmp("rdoNoTimesLimit").setValue(false);
                Ext.getCmp("rdoByOrder").setValue(false);
                Ext.getCmp("rdoByMember").setValue(true);
                Ext.getCmp("num_limit").setValue(row.data.num_limit);
                break;
        }

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


        //修改時對payment賦值

        paymentStore.load({
            callback: function () {
                var paymentIDs = row.data.payment_code.toString().split(',');
                var arrTemp = new Array();
                for (var i = 0; i < paymentIDs.length; i++) {
                    arrTemp.push(paymentStore.getAt(paymentStore.find("parameterCode", paymentIDs[i])));
                }
                Ext.getCmp('payment').setValue(arrTemp);

            }
        })

    }
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


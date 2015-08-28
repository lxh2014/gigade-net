
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
    var BONUS_SATE = 0;
    VipGroupStore.load();
    YunsongStore.load();
    ShopClassStore.load();
    if (RowID != null) {
        BONUS_SATE = RowID.data.bonus_state;
        promoID = RowID.data.id;
        prodCateID = RowID.data.category_id;
        websiteID = RowID.data.site == "" ? 1 : RowID.data.site;
        conditionID = RowID.data.condition_id;
    }
    else {
        SiteStore.load();
        paymentStore.load();
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
    //點/點和+金Store
    var PointsStore = Ext.create('Ext.data.Store', {
        fields: ['Points_id', 'Points_name'],
        data: [
            { Points_id: '1', Points_name: "點" },
            { Points_id: '2', Points_name: "點+金" },
            { Points_id: '3', Points_name: "金" },
            { Points_id: '4', Points_name: "比例固定" },
            { Points_id: '5', Points_name: "非固定" }
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
                        if (Ext.getCmp("freight_price").getValue() != 0 && Ext.getCmp("freight_price").getValue() != null) {
                            event_type = "試吃";
                            condition_name = "G3";

                        } else if (Ext.getCmp("ra2").getValue() == true) {
                            event_type = "紅利折抵";
                            condition_name = "G4";
                        } else if (Ext.getCmp("ra0").getValue() == true) {
                            event_type = "一般";
                            condition_name = "G0";
                        }
                        Ext.Ajax.request({
                            url: '/PromotionsAmountGift/TryEatAndDiscountFirstSaveGift',
                            method: 'post',
                            params: {
                                name: Ext.getCmp("name").getValue(),
                                bonus_state: BONUS_SATE,
                                desc: Ext.getCmp("event_desc").getValue(),
                                Points: Ext.getCmp("Points").getValue(),
                                freight_price: Ext.getCmp("freight_price").getValue(),
                                vendor_coverage: Ext.htmlEncode(Ext.getCmp('vendor_coverage').getValue()),
                                event_type_name: event_type,
                                point: Ext.htmlEncode(Ext.getCmp('point').getValue()),
                                dollar: Ext.htmlEncode(Ext.getCmp('dollar').getValue())
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    promoID = result.id;
                                    prodCateID = result.cateID;
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
                        if (Ext.getCmp("freight_price").getValue() != 0 && Ext.getCmp("freight_price").getValue() != null) {
                            event_type = "試吃";
                            condition_name = "G3";

                        } else if (Ext.getCmp("ra2").getValue() == true) {
                            event_type = "紅利折抵";
                            condition_name = "G4";
                        }
                        else if (Ext.getCmp("ra0").getValue() == true) {
                            event_type = "一般";
                            condition_name = "G0";
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
                               fieldLabel: '一般',
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
                                   id: "ra0",
                                   checked: true,
                                   listeners: {
                                       change: function (radio, newValue, oldValue) {
                                           var amountCP = Ext.getCmp("freight_price");
                                           var quantityCP = Ext.getCmp("Points");
                                           if (newValue) {
                                               BONUS_SATE = 0;
                                               amountCP.setValue("0");
                                               amountCP.allowBlank = true;
                                               amountCP.setValue("");
                                               amountCP.setDisabled(true);
                                               quantityCP.setValue("0");
                                               quantityCP.allowBlank = true;
                                               quantityCP.setValue("");
                                               quantityCP.setDisabled(true);

                                           }
                                       }
                                   }
                               }
                               ]

                           }
                  ,
                     {
                         xtype: 'fieldcontainer',
                         fieldLabel: '試吃',
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
                             inputValue: "freight_price",
                             id: "ra1",
                             width: 20,
                             listeners: {
                                 change: function (radio, newValue, oldValue) {
                                     var amountCP = Ext.getCmp("freight_price");
                                     var quantityCP = Ext.getCmp("Points");
                                     var point = Ext.getCmp("point");
                                     var dollar = Ext.getCmp("dollar");
                                     if (newValue) {
                                         BONUS_SATE = 1;
                                         amountCP.setDisabled(false);
                                         amountCP.allowBlank = false;
                                         quantityCP.setValue("0");
                                         quantityCP.allowBlank = true;
                                         quantityCP.setValue("");
                                         quantityCP.setDisabled(true);
                                         point.setValue("0");
                                         point.allowBlank = false;
                                         point.hide();
                                         dollar.setValue("0");
                                         dollar.allowBlank = false;
                                         dollar.hide();


                                     }
                                 }
                             }

                         },
                          {
                              xtype: 'fieldcontainer',
                              combineErrors: true,
                              width: 150,
                              layout: 'hbox',
                              defaults: {
                                  hideLabel: true
                              },
                              items: [
                          {

                              xtype: 'displayfield',
                              id: 'yunfei',
                              value: '運費:'
                          },
                         {
                             xtype: "numberfield",
                             id: 'freight_price',
                             labelWidth: 10,
                             disabled: true,
                             flex: 1,
                             name: 'freight_price',
                             allowBlank: false,
                             minValue: 0
                         }
                              ]
                          }
                         ]
                     },
                     {
                         xtype: 'fieldcontainer',
                         fieldLabel: '紅利折抵',
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
                                     var amountCP = Ext.getCmp("freight_price");
                                     var quantityCP = Ext.getCmp("Points");
                                     if (newValue) {
                                         BONUS_SATE = 2;
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
                             xtype: 'combobox',
                             fieldLabel: '',
                             id: 'Points',
                             queryMode: 'local',
                             name: 'Points',
                             allowBlank: false,
                             disabled: true,
                             editable: false,
                             typeAhead: true,
                             forceSelection: false,
                             store: PointsStore,
                             displayField: 'Points_name',
                             valueField: 'Points_id',
                             emptyText: SELECT,
                             listeners: {
                                 select: function (a, b, c) {
                                     var compoint = Ext.getCmp("Points");
                                     var point = Ext.getCmp("point");
                                     var dollar = Ext.getCmp("dollar");
                                     if (compoint.getValue() == 4) {
                                         point.setValue("0");
                                         point.allowBlank = false;
                                         point.show();
                                         dollar.setValue("0");
                                         dollar.allowBlank = false;
                                         dollar.show();
                                     }
                                     else if (compoint.getValue() == 5) {
                                         point.setValue("0");
                                         point.allowBlank = true;
                                         point.hide();
                                         dollar.setValue("0");
                                         dollar.allowBlank = false;
                                         dollar.show();
                                     }
                                     else if (compoint.getValue() == 1 || compoint.getValue() == 2 || compoint.getValue() == 3) {
                                         point.setValue("0");
                                         point.allowBlank = true;
                                         point.hide();
                                         dollar.setValue("0");
                                         dollar.allowBlank = true;
                                         dollar.hide();
                                     }
                                 }
                             }

                         }

                         ]

                     }
                  ,
                      {
                          xtype: "numberfield",
                          fieldLabel: "元",
                          id: 'dollar',
                          name: 'dollar',
                          allowBlank: true,
                          padding: '10 0 5 0',
                          minValue: 0,
                          hidden: true
                      },
                    {
                        xtype: "numberfield",
                        fieldLabel: "點數",
                        id: 'point',
                        name: 'point',
                        allowBlank: true,
                        padding: '10 0 5 0',
                        minValue: 0,
                        hidden: true
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
                                    hidden: false,
                                    id: 'group_id',
                                    //allowBlank: false,
                                    name: 'group_name',
                                    hiddenName: 'group_id',
                                    store: VipGroupStore,
                                    displayField: 'group_name',
                                    valueField: 'group_id',
                                    lastQuery: '',
                                    typeAhead: true,
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
                                            var giftMundane = Ext.getCmp("gift_mundane");
                                            if (newValue) {
                                                numLimit.setValue(0);
                                                numLimit.setDisabled(true);
                                                giftMundane.setValue(0);
                                                giftMundane.setDisabled(true);
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
                                            var giftMundane = Ext.getCmp("gift_mundane");
                                            if (newValue) {
                                                numLimit.setValue(0);
                                                numLimit.setDisabled(false);
                                                giftMundane.setValue(0);
                                                giftMundane.setDisabled(false);
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
                                             var giftMundane = Ext.getCmp("gift_mundane");
                                             if (newValue) {
                                                 numLimit.setValue(0);
                                                 numLimit.setDisabled(false);
                                                 giftMundane.setValue(0);
                                                 giftMundane.setDisabled(false);
                                             }
                                         }
                                     }
                                 }
                            ]
                        },
                        {
                            xtype: 'numberfield',
                            fieldLabel: '試吃次數',
                            name: 'num_limit',
                            id: 'num_limit',
                            minValue: 0,
                            value: 0,
                            disabled: true
                        },
                      {
                          xtype: 'numberfield',
                          fieldLabel: '限量件數',
                          name: 'gift_mundane',
                          id: 'gift_mundane',
                          minValue: 0,
                          value: 0,
                          disabled: true
                      },
                        {
                            xtype: 'radiogroup',
                            hidden: false,
                            id: 'repeat',
                            name: 'repeat',
                            fieldLabel: '是否重複送',
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
                },
                {
                    xtype: 'combobox', //Wibset
                    fieldLabel: '購物車',
                    id: 'delivery_category',
                    name: 'delivery_category',
                    hiddenName: 'delivery_category',
                    colName: 'delivery_category',
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
               url: ' /PromotionsAmountGift/TryEatAndDiscountSecondSaveGift',
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

                       if (condition_id == "" && Ext.getCmp("group_id").getValue() == null) {
                           Ext.Msg.alert(INFORMATION, USERCONDITIONERROR);
                           return;
                       }
                       var Xuhao = "";
                       var activeNow = 0;
                       var startdate = "";
                       var enddate = "";
                       //var gift_type = Ext.htmlEncode(activeTypetabPanel.getActiveTab().title);
                       var boolValid = true;
                       if (startdate > enddate) {
                           Ext.Msg.alert(INFORMATION, USERDATETIP);
                           return;
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
                                   bonus_state: BONUS_SATE,
                                   event_type: condition_name,
                                   url_by: Ext.htmlEncode(Ext.getCmp("url_by").getValue().url_by),
                                   banner_url: Ext.htmlEncode(Ext.getCmp("banner_url").getValue()),
                                   group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                                   condition_id: condition_id,
                                   freight_price: Ext.htmlEncode(Ext.getCmp("freight_price").getValue()),
                                   Points: Ext.htmlEncode(Ext.getCmp("Points").getValue()),
                                   sclass_id: Ext.htmlEncode(Ext.getCmp('class_id').getValue()),
                                   sbrand_id: document.getElementsByName('brand_name')[0].value,
                                   allClass: Ext.htmlEncode(Ext.getCmp("allClass").getValue()),
                                   count_by: Ext.htmlEncode(Ext.getCmp('count_by').getValue().count_by),
                                   numLimit: Ext.htmlEncode(Ext.getCmp('num_limit').getValue()),
                                   gift_mundane: Ext.htmlEncode(Ext.getCmp('gift_mundane').getValue()),
                                   IsRepeat: Ext.htmlEncode(Ext.getCmp('repeat').getValue().IsRepeat),
                                   frieghttype: Ext.htmlEncode(Ext.getCmp('freight').getValue()),
                                   devicename: Ext.htmlEncode(Ext.getCmp("deviceName").getValue().deviceName),
                                   payment: Ext.htmlEncode(Ext.getCmp('payment').getValue()),
                                   start_time: Ext.htmlEncode(Ext.getCmp('startdate').getRawValue()),
                                   end_time: Ext.htmlEncode(Ext.getCmp('enddate').getRawValue()),
                                   site: Ext.htmlEncode(Ext.getCmp('site').getValue()),
                                   delivery_category: Ext.htmlEncode(Ext.getCmp('delivery_category').getValue()),
                                   Xuhao: Ext.htmlEncode(Xuhao),
                                   activeNow: Ext.htmlEncode(activeNow),
                                   startdate: Ext.htmlEncode(startdate),
                                   enddate: Ext.htmlEncode(enddate),
                                   vendor_coverage: Ext.htmlEncode(Ext.getCmp('vendor_coverage').getValue()),
                                   point: Ext.htmlEncode(Ext.getCmp('point').getValue()),
                                   dollar: Ext.htmlEncode(Ext.getCmp('dollar').getValue())
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
                                           var allClass = Ext.getCmp("allClass");
                                           var photo = Ext.getCmp("photo");
                                           if (newValue) {
                                               var banner_image = Ext.getCmp("banner_url");
                                               banner_image.setValue(linkPath + GetEventId(condition_name, promoID));
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
                                           var allClass = Ext.getCmp("allClass");
                                           var photo = Ext.getCmp("photo");
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
                   labelWidth: 80,
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
                   labelWidth: 80,
                   msgTarget: 'side',
                   anchor: '95%',
                   buttonText: SELECT_IMG,
                   submitValue: true,
                   allowBlank: true,
                   fileUpload: true,
                   hidden: true
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
        title: "試吃/紅利折抵",
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
        if (row.data.class_id != 0 && row.data.brand_id != 0) {
            Ext.getCmp('brand_id').setDisabled(false);
            classBrandStore.load({
                params: {
                    topValue: Ext.getCmp("class_id").getValue()
                }
            });
        }

        if (row.data.freight_price == 0) {
            Ext.getCmp("freight_price").setValue("")
        }
        if (row.data.condition_id != 0) {

            Ext.getCmp('us1').setValue(false);
            Ext.getCmp('us2').setValue(true);

        }
        else {
            Ext.getCmp('us1').setValue(true);
            Ext.getCmp('us2').setValue(false);
            if (row.data.group_name == "") {
                Ext.getCmp('group_id').setValue(0);
            }
        }
        if (row.data.payment == "") {

            Ext.getCmp("payment").setValue(DEVICE_1);
        }

        switch (row.data.devicename) {
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
        if (row.data.class_id == "0") {
            Ext.getCmp('class_id').setValue("");
        }
        if (row.data.product_id == 999999 && row.data.brand_id == 0) {
            Ext.getCmp("allClass").setValue(true);
        }

        switch (row.data.bonus_state) {
            case 0:
                Ext.getCmp("ra0").setValue(true);
                Ext.getCmp("ra1").setDisabled(true);
                Ext.getCmp("ra2").setDisabled(true);
                Ext.getCmp("Points").setDisabled(true);
                Ext.getCmp("freight_price").setDisabled(true);

                break;
            case 1:
                Ext.getCmp("ra1").setValue(true);
                Ext.getCmp("ra0").setDisabled(true);
                Ext.getCmp("ra2").setDisabled(true);
                Ext.getCmp("Points").setDisabled(true);
                Ext.getCmp("freight_price").setDisabled(false);
                Ext.getCmp("freight_price").setValue(row.data.freight_price);
                break;
            case 2:
                Ext.getCmp("ra2").setValue(true);
                Ext.getCmp("ra0").setDisabled(true);
                Ext.getCmp("ra1").setDisabled(true);
                Ext.getCmp("freight_price").setDisabled(true);
                Ext.getCmp("Points").setValue(row.data.dividend);
                Ext.getCmp("Points").setDisabled(false);
                break;

        }

        if (row.data.dividend == 4) {
            Ext.getCmp("point").show();
            Ext.getCmp("dollar").show();
        }
        else if (row.data.dividend == 5) {
            Ext.getCmp("dollar").show();
        }
        if (row.data.freight == "") {
            Ext.getCmp("freight").setValue(0);
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
                Ext.getCmp("gift_mundane").setValue(row.data.gift_mundane);
                break;
            case 3:
                Ext.getCmp("rdoNoTimesLimit").setValue(false);
                Ext.getCmp("rdoByOrder").setValue(false);
                Ext.getCmp("rdoByMember").setValue(true);
                Ext.getCmp("num_limit").setValue(row.data.num_limit);
                Ext.getCmp("gift_mundane").setValue(row.data.gift_mundane);
                break;
        }
        if (row.data.url_by == 0) {
            Ext.getCmp("bt2").setValue(true);
            Ext.getCmp("banner_url").setValue("");
        }
        else if (row.data.url_by == 1) {
            Ext.getCmp("bt1").setValue(true);
        }
        //修改時對site賦值


        SiteStore.load({
            callback: function () {
                var siteIDs = row.data.site.toString().split(',');
                var dcIDs = row.data.delivery_category.toString().split(',');
                var arrTemp = new Array();
                for (var i = 0; i < siteIDs.length; i++) {
                    arrTemp.push(SiteStore.getAt(SiteStore.find("Site_Id", siteIDs[i])));
                }
                Ext.getCmp('site').setValue(arrTemp);
                var dcarr = new Array();
                for (var i = 0; i < dcIDs.length; i++) {
                    dcarr.push(SiteStore.getAt(SiteStore.find("Site_Name", dcIDs[i])));
                }
                Ext.getCmp('delivery_category').setValue(dcarr);
            }
        })

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


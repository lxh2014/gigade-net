Ext.define('gigade.Paper', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "paperID", type: "int" },
    { name: "paperName", type: "string" }]
});
var PaperStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Paper',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Paper/GetPaperList?isPage=false',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data' 
        }
    }
});
var frontCateStore = Ext.create('Ext.data.TreeStore', {
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/ProductList/GetFrontCatagory',
        actionMethods: 'post'
    },
    root: {
        expanded: true,
        checked: false,
        children: [

        ]
    }
});
frontCateStore.load();

function editFunction(RowID, PromoAmountTrialStore) {
    //前台分類store


    SiteStore.load();
    VipGroupStore.load();
    var conditionID = ""; //保存條件id
    var condiCount = 0;
    var linkPath = "http://www.gigade100.com/promotion/combo_promotion.php?event_id="; //保存图片链接的地址
    var currentPanel = 0;
    var Trial_id = "";
    var websiteID = 1;
    var event_type = "T1";
    var Event_id = "";
    var promoID = "";
    var prodCateID = "";
    if (RowID != null) {
        Trial_id = RowID.data.id;
        event_type = RowID.data.event_type;
        Event_id = RowID.data.event_id;
        websiteID = RowID.data.site == "" ? 1 : RowID.data.site;
        conditionID = RowID.data.condition_id;
    }
    var navigate = function (panel, direction) {
        var layout = panel.getLayout();
        if ('next' == direction) {
            if (currentPanel == 0) {//首頁時進行第一步保存
                var ffrom = firstForm.getForm();
                if (!RowID && Event_id == "") {
                    if (ffrom.isValid()) {
                        if (Ext.htmlEncode(Ext.getCmp("event_type").getValue().eventtype) == 1) {
                            event_type = "T1";

                        } else if (Ext.htmlEncode(Ext.getCmp("event_type").getValue().eventtype) == 2) {
                            event_type = "T2";
                        }
                        Ext.Ajax.request({
                            url: '/PromotionsAmountTrial/SaveTrial',
                            method: 'post',
                            params: {
                                name: Ext.getCmp("name").getValue(),
                                event_type: Ext.htmlEncode(Ext.getCmp("event_type").getValue().eventtype),
                                paper_id: Ext.htmlEncode(Ext.getCmp('paper_id').getValue()),
                                start_date: Ext.htmlEncode(Ext.getCmp('start_date').getRawValue()),
                                end_date: Ext.htmlEncode(Ext.getCmp('end_date').getRawValue()),
                                event_desc: Ext.htmlEncode(Ext.getCmp('event_desc').getValue())
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    Trial_id = result.id;
                                    Event_id = result.event_id;
                                    Ext.getCmp("url").setValue(linkPath + Event_id);

                                    layout[direction]();
                                    currentPanel++;
                                    if (!layout.getNext()) {
                                        Ext.getCmp('move-next').hide();
                                    }
                                    else {
                                        Ext.getCmp('move-prev').show();
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
                    else { Ext.getCmp('move-prev').hide(); return; }
                }
                else {//編輯
                    if (Event_id.substr(0, 2) == "T1") {
                        Ext.getCmp("eatdisplay").show();
                        Ext.getCmp("usedisplay").hide();
                    } else {
                        Ext.getCmp("eatdisplay").hide();
                        Ext.getCmp("usedisplay").show();
                    }
                    if (ffrom.isValid()) {
                        Ext.getCmp('move-prev').show();
                        layout[direction]();
                        currentPanel++;
                    }

                }
            } else {
                var forms = secondForm.getForm();
                if (Event_id.substr(0, 2) == "T1") {
                    Ext.getCmp("eatdisplay").show();
                    Ext.getCmp("usedisplay").hide();
                } else {
                    Ext.getCmp("eatdisplay").hide();
                    Ext.getCmp("usedisplay").show();
                }
                if (forms.isValid()) {
                    layout[direction]();
                    currentPanel++;
                    Ext.getCmp('move-prev').show();
                    Ext.getCmp('move-next').hide();
                }
            }
        }
        else {
            layout[direction]();
            currentPanel--;
            Ext.getCmp('move-prev').show();
            Ext.getCmp('move-next').show();
            if (currentPanel == 0) {
                Ext.getCmp('move-prev').hide();
            }
        }
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
            fieldLabel: '<span style="font-size:12px; color:#F00;">※</span>' + ACTIVENAME,
            xtype: 'textfield',
            id: 'name',
            name: 'name',
            allowBlank: false
        },
        {
            xtype: 'radiogroup',
            hidden: false,
            id: 'event_type',
            name: 'event_type',
            fieldLabel: '<span style="font-size:12px; color:#F00;">※</span>' + ACTIVETYPE,
            colName: 'eventtype',
            width: 400,
            defaults: {
                name: 'eventtype',
                margin: '0 8 0 0'
            },
            columns: 2,
            vertical: true,
            items: [
            { boxLabel: TRYEAT, id: 'tryeat', inputValue: '1', checked: true },
            { boxLabel: TRYUSE, id: 'tryuse', inputValue: '2' }
            ]
        },
         {
             xtype: 'textfield',
             fieldLabel: '<span style="font-size:12px; color:#F00;">※</span>' + WEBURL,
             name: 'url',
             id: 'url',
             submitValue: true,
             vtype: 'url',
             hidden: true,
             editable: false,
             readOnly: true
         },
        {
            xtype: 'combobox', //Wibset
            fieldLabel: '<span style="font-size:12px; color:#F00;">※</span>' + PAPER,
            id: 'paper_id',
            name: 'paper_id',
            hiddenName: 'paper_id',
            colName: 'paper_id',
            allowBlank: false,
            editable: false,
            store: PaperStore,
            displayField: 'paperName',
            valueField: 'paperID',
            typeAhead: true,
            lastQuery: '',
            forceSelection: false,
            emptyText: SELECT,
            queryMode: 'local'
        },


        {
            xtype: "datetimefield",
            fieldLabel: '<span style="font-size:12px; color:#F00;">※</span>' + BEGINTIME,
            editable: false,
            id: 'start_date',
            name: 'start_date',
            anchor: '95%',
            format: 'Y-m-d H:i:s',
            width: 150,
            allowBlank: false,
            submitValue: true,
            value: Tomorrow(),
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("start_date");
                    var end = Ext.getCmp("end_date");
                    if (end.getValue() < start.getValue()) {
                        Ext.Msg.alert(INFORMATION, TIMETIP);
                    }
                }
            }
        },
        {
            xtype: "datetimefield",
            fieldLabel: '<span style="font-size:12px; color:#F00;">※</span>' + ENDTIME,
            editable: false,
            id: 'end_date',
            anchor: '95%',
            name: 'end_date',
            format: 'Y-m-d H:i:s',
            allowBlank: false,
            submitValue: true,
            value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() + 1)),
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("start_date");
                    var end = Ext.getCmp("end_date");
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
            fieldLabel: ACTIVEDESC,
            xtype: 'textareafield',
            anchor: '95%',
            id: 'event_desc',
            name: 'event_desc'
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
                                   lastQuery: '',
                                   name: 'group_name',
                                   hiddenName: 'group_id',
                                   store: VipGroupStore,
                                   displayField: 'group_name',
                                   valueField: 'group_id',
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
                                           showUserSetForm(event_type, conditionID, "userInfo");
                                       } else {
                                           showUserSetForm(event_type, condition_id, "userInfo");
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
                                 numLimit.setValue(1);
                                 numLimit.setDisabled(false);
                                 giftMundane.setValue(1);
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
                      checked: true,
                      listeners: {
                          change: function (radio, newValue, oldValue) {
                              var numLimit = Ext.getCmp("num_limit");
                              var giftMundane = Ext.getCmp("gift_mundane");
                              if (newValue) {
                                  numLimit.setValue(1);
                                  numLimit.setDisabled(false);
                                  giftMundane.setValue(1);
                                  giftMundane.setDisabled(false);
                              }
                          }
                      }
                  }
             ]
         },
          {
              xtype: 'fieldcontainer',
              combineErrors: true,
              layout: 'hbox',
              items: [
         {
             xtype: 'displayfield',
             value: '試吃次數:',
             id: 'eatdisplay',
             width: 105
         },
         {
             xtype: 'displayfield',
             value: '試用次數:',
             id: 'usedisplay',
             hidden: true,
             width: 105
         },
         {
             xtype: 'numberfield',
             name: 'num_limit',
             id: 'num_limit',
             minValue: 0,
             value: 1
         }
              ]
          },
       {
           xtype: 'numberfield',
           fieldLabel: '限量件數',
           name: 'gift_mundane',
           id: 'gift_mundane',
           minValue: 0,
           value: 1
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
                id: 'freight_type',
                name: 'freight_type',
                lastQuery: '',
                hiddenName: 'typeName',
                colName: 'typeName',
                displayField: 'type_name',
                valueField: 'type_id',
                typeAhead: true,
                forceSelection: false,
                emptyText: SELECT,
                value: 0
            },
               {
                   xtype: 'radiogroup',
                   hidden: false,
                   id: 'device_name',
                   name: 'device_name',
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

               xtype: 'combobox', //Wibset
               allowBlank: false,
               editable: false,
               fieldLabel: WEBSITESET,
               hidden: false,
               id: 'site',
               name: 'website',
               hiddenName: 'website',
               colName: 'site',
               store: SiteStore,
               displayField: 'Site_Name',
               valueField: 'Site_Id',
               typeAhead: true,
               forceSelection: false,
               queryMode: 'local',
               value: 1,
               multiSelect: true, //多選
               emptyText: SELECT,
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
        url: ' /PromotionsAmountTrial/UpdateTrial',
        defaultType: 'textfield',
        layout: 'anchor',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
        {
            xtype: 'fieldcontainer',
            defaults: {
                labelWidth: 80
            },
            combineErrors: true,
            layout: 'hbox',
            items: [
            {
                xtype: 'numberfield',
                fieldLabel: '<span style="font-size:12px; color:#F00;">※</span>' + "活動商品",
                allowBlank: true,
                readOnly: true,
                id: 'product_id',
                name: 'product_id',
                width: 140,
                minValue: 0
            },

              {
                  xtype: 'numberfield',
                  fieldLabel: '<span style="font-size:12px; color:#F00;">※</span>' + "販售商品",
                  allowBlank: false,
                  readOnly: true,
                  id: 'sale_product_id',
                  name: 'sale_product_id',
                  width: 140,
                  margin: '0 5 0 5',
                  minValue: 0
              },
            {//商品設定
                xtype: 'button',
                text: PRODSELECT,
                id: 'btnProdId',
                name: 'btnProdId',
                width: 60,
                minValue: 0,
                handler: function () {
                    var cate_id = Ext.getCmp('category_id');
                    var product_id = Ext.getCmp('product_id');
                    var sale_product_id = Ext.getCmp('sale_product_id');
                    var sale_product_name = Ext.getCmp('sale_product_name');
                    CategoryProductShow(websiteID, cate_id, product_id, sale_product_id, sale_product_name);
                }
            }
            ]
        },

        {//商品名稱
            xtype: 'textfield',
            fieldLabel: '<span style="font-size:12px; color:#F00;">※</span>' + PRODNAME,
            allowBlank: false,
            id: 'product_name',
            name: 'product_name',
            labelWidth: 80
        }, {
            xtype: 'displayfield',
            fieldLabel: "販售商品名稱",
            allowBlank: true,
            readOnly: true,
            hidden: true,
            allowBlank: false,
            id: 'sale_product_name',
            name: 'sale_product_name',
            width: 180,
            labelWidth: 80
        },
        {//類別id
            xtype: 'textfield',
            fieldLabel: CATEGORYID,
            editable: false,
            hidden: true,
            id: 'category_id',
            name: 'category_id',
            minValue: 0,
            value: 0,
            labelWidth: 80,
            readOnly: true
        },
        {//類別名稱
            xtype: 'displayfield',
            fieldLabel: CATEGORYNAME,
            editable: false,
            width: 180,
            labelWidth: 80,
            hidden: true,
            id: 'category_name',
            name: 'category_name'
        },
        {//品牌id
            xtype: 'textfield',
            fieldLabel: BRANDID,
            editable: false,
            hidden: true,
            labelWidth: 80,
            id: 'brand_id',
            name: 'brand_id',
            minValue: 0,
            value: 0,
            readOnly: true
        },
        {//品牌名稱
            xtype: 'displayfield',
            fieldLabel: BRANDNAME,
            hidden: true,
            editable: false,
            width: 200,
            labelWidth: 80,
            id: 'brand_name',
            name: 'brand_name'

        },
        {//商品圖片路徑
            xtype: 'displayfield',
            fieldLabel: PRODIMGFILE,
            hidden: true,
            editable: false,
            width: 200,
            id: 'prod_file',
            name: 'prod_file'

        },
        {//商品图片
            xtype: 'filefield',
            name: 'product_img',
            id: 'product_img',
            fieldLabel: PRODIMG,
            labelWidth: 80,
            msgTarget: 'side',
            anchor: '95%',
            buttonText: SELECT_IMG,
            submitValue: true,
            allowBlank: true,
            fileUpload: true,
            listeners: {
                change: function (field, value) {
                    if (RowID != null) {
                        if (value.indexOf("\:\\") > 0 || value != RowID.data.product_img) {
                            Ext.getCmp("prod_file").setValue("");
                        }
                    } else {
                        Ext.getCmp("prod_file").setValue("");
                    }
                }
            }
        },

        {//活动小图片
            xtype: 'filefield',
            name: 'event_img_small',
            id: 'event_img_small',
            fieldLabel: EVENTIMGSMALL,
            labelWidth: 80,
            msgTarget: 'side',
            anchor: '95%',
            buttonText: SELECT_IMG,
            submitValue: true,
            allowBlank: true,
            fileUpload: true

        },

        {//活动图片
            xtype: 'filefield',
            name: 'event_img',
            id: 'event_img',
            fieldLabel: EVENTEDM,
            labelWidth: 80,
            msgTarget: 'side',
            anchor: '95%',
            buttonText: SELECT_IMG,
            submitValue: true,
            allowBlank: true,
            fileUpload: true

        },
        {
            xtype: "numberfield",
            fieldLabel: '<span style="font-size:12px; color:#F00;">※</span>' + MARKETPRICE,
            id: 'market_price',
            name: 'market_price',
            allowBlank: false,
            minValue: 0,
            maxValue: 65535,
            value: 0,
            submitValue: true
        },
        {
            xtype: "numberfield",
            fieldLabel: '<span style="font-size:12px; color:#F00;">※</span>' + SHOWNUMBER,
            id: 'show_number',
            name: 'show_number',
            allowBlank: false,
            minValue: 0,
            maxValue: 65535,
            value: 0,
            submitValue: true
        },
        {
            xtype: "numberfield",
            fieldLabel: '<span style="font-size:12px; color:#F00;">※</span>' + APPLYLIMIT,
            id: 'apply_limit',
            name: 'apply_limit',
            allowBlank: false,
            minValue: 0,
            maxValue: 65535,
            value: 0,
            submitValue: true
        },
        {
            xtype: "numberfield",
            fieldLabel: '<span style="font-size:12px; color:#F00;">※</span>' + APPLYSUM,
            id: 'apply_sum',
            name: 'apply_sum',
            allowBlank: false,
            minValue: 0,
            value: 0,
            submitValue: true
        }
        ],
        buttons: [
        {
            text: SAVE,
            formBind: true,
            disabled: true,
            id: 'pSave',
            handler: function () {
             
                var form = this.up('form').getForm();
                if (condition_id == "" && Ext.getCmp("group_id").getValue() == null) {
                    Ext.Msg.alert(INFORMATION, USERCONDITIONERROR);
                    return;
                }
                if (Ext.getCmp("product_id").getValue() == "" || Ext.getCmp("product_id").getValue() == null || Ext.getCmp("product_id").getValue() == 0) {
                    Ext.Msg.alert(INFORMATION, "請選擇商品");
                    return;
                }
                if (Ext.getCmp("apply_limit").getValue() < Ext.getCmp("apply_sum").getValue()) {
                    Ext.Msg.alert(INFORMATION, APPLYTIP);
                    return;
                }
                if (form.isValid()) {
                    this.disable();
                    form.submit({
                        params: {
                            isEdit: RowID,
                            trial_id: Trial_id,
                            name: Ext.getCmp("name").getValue(),
                            event_type: Ext.htmlEncode(Ext.getCmp("event_type").getValue().eventtype),
                            event_url: Ext.htmlEncode(Ext.getCmp('url').getValue()),
                            paper_id: Ext.htmlEncode(Ext.getCmp('paper_id').getValue()),
                            start_date: Ext.htmlEncode(Ext.getCmp('start_date').getRawValue()),
                            end_date: Ext.htmlEncode(Ext.getCmp('end_date').getRawValue()),
                            event_desc: Ext.htmlEncode(Ext.getCmp('event_desc').getValue()),
                            group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                            condition_id: condition_id,
                            count_by: Ext.htmlEncode(Ext.getCmp('count_by').getValue().count_by),
                            numLimit: Ext.htmlEncode(Ext.getCmp('num_limit').getValue()),
                            gift_mundane: Ext.htmlEncode(Ext.getCmp('gift_mundane').getValue()),
                            IsRepeat: Ext.htmlEncode(Ext.getCmp('repeat').getValue().IsRepeat),
                            freight_type: Ext.htmlEncode(Ext.getCmp('freight_type').getValue()),
                            device_name: Ext.htmlEncode(Ext.getCmp("device_name").getValue().deviceName),
                            site: Ext.htmlEncode(Ext.getCmp('site').getValue()),
                            sale_product_id: Ext.htmlEncode(Ext.getCmp('sale_product_id').getValue()),
                            product_id: Ext.htmlEncode(Ext.getCmp('product_id').getValue()),
                            product_name: Ext.htmlEncode(Ext.getCmp('product_name').getValue()),
                            category_id: Ext.htmlEncode(Ext.getCmp('category_id').getValue()),
                            brand_id: Ext.htmlEncode(Ext.getCmp('brand_id').getValue()),
                            market_price: Ext.htmlEncode(Ext.getCmp('market_price').getValue()),
                            show_number: Ext.htmlEncode(Ext.getCmp('show_number').getValue()),
                            apply_limit: Ext.htmlEncode(Ext.getCmp('apply_limit').getValue()),
                            apply_sum: Ext.htmlEncode(Ext.getCmp('apply_sum').getValue()),
                            prod_file: Ext.htmlEncode(Ext.getCmp('prod_file').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                PromoAmountTrialStore.load();
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
            hidden: true,
            handler: function (btn) {
                navigate(btn.up("panel"), "prev");
            }
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
        title: ACTIVITY,
        iconCls: RowID ? "icon-user-edit" : "icon-user-add",
        width: 410,
        y: 100,
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

    function initForm(row) {
        //site賦值
        if (row.data.site != "") {
            var siteIDs = row.data.site.toString().split(',');

            var combobox = Ext.getCmp('site');
            //var store = combobox.store;
            var arrTemp = new Array();
            for (var i = 0; i < siteIDs.length; i++) {
                //arrTemp.push(store.getAt(store.find("Site_Id", siteIDs[i])));
                arrTemp.push(siteIDs[i]);
            }
            combobox.setValue(arrTemp);
        }
        //活動類型
        switch (row.data.event_type) {
            case "T1":
                Ext.getCmp("tryeat").setValue(true);
                Ext.getCmp("tryuse").setDisabled(true);
                break;
            case "T2":
                Ext.getCmp("tryuse").setValue(true);
                Ext.getCmp("tryeat").setDisabled(true);
                break;

        }

        var numLimit = Ext.getCmp("num_limit");
        var giftMundane = Ext.getCmp("gift_mundane");
        if (row.data.count_by == "1") {
            Ext.getCmp("rdoNoTimesLimit").setValue(true);
            numLimit.setValue(0);
            numLimit.setDisabled(true);
            giftMundane.setValue(0);
            giftMundane.setDisabled(true);
        }
        if (row.data.count_by == "2") {
            Ext.getCmp("rdoByOrder").setValue(true);
            numLimit.setDisabled(false);
            giftMundane.setDisabled(false);
            numLimit.setValue(row.data.num_limit)
            giftMundane.setValue(row.data.gift_mundane);
        }
        if (row.data.count_by == "3") {
            Ext.getCmp("rdoByMember").setValue(true);
            //numLimit.setValue(0);
            numLimit.setDisabled(false);
            // giftMundane.setValue(0);
            giftMundane.setDisabled(false);
            numLimit.setValue(row.data.num_limit);
            giftMundane.setValue(row.data.gift_mundane);
        }
        //裝置
        switch (row.data.device_name) {
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

        //會員條件
        if (row.data.condition_id != 0) {
            Ext.getCmp('us1').setValue(false);
            Ext.getCmp('us2').setValue(true);
        }
        else {
            Ext.getCmp('us1').setValue(true);
            Ext.getCmp('us2').setValue(false);
            if (row.data.group_id == "" || row.data.group_id == "0" || row.data.group_id == 0) {
                Ext.getCmp('group_id').setValue("0");
            }
        }
        if (row.data.sale_productid != 0) {
            Ext.getCmp("sale_product_id").setValue(row.data.sale_productid);
            Ext.getCmp("sale_product_name").show();
            Ext.getCmp("sale_product_name").setValue(row.data.sale_product_name);
        }

        //圖片
        var img, imgUrl;
        if (row.data.event_img_small.toString() != "") {
            img = row.data.event_img_small.toString();
            imgUrl = img.substring(img.lastIndexOf("\/") + 1);
            Ext.getCmp('event_img_small').setRawValue(imgUrl);
        }
        if (row.data.event_img.toString() != "") {
            img = row.data.event_img.toString();
            imgUrl = img.substring(img.lastIndexOf("\/") + 1);
            Ext.getCmp('event_img').setRawValue(imgUrl);
        }
        if (row.data.product_img.toString() != "") {
            img = row.data.product_img.toString();
            var regex = /^[T1|T2].*$/;
            if (!regex.test(img)) {
                Ext.getCmp("prod_file").setValue(img);
            }
            imgUrl = img.
            substring(img.lastIndexOf("\/") + 1);
            Ext.getCmp('product_img').setRawValue(imgUrl);
        }
        if (row.data.category_id != "0" && row.data.category_id != "") {
            Ext.getCmp('category_id').setValue(row.data.category_id);
            Ext.getCmp('category_name').setValue(row.data.category_name);
            Ext.getCmp('category_name').show();
        }

        if (row.data.brand_id != 0 && row.data.brand_id != "") {
            Ext.getCmp('brand_id').setValue(row.data.brand_id);
            Ext.getCmp('brand_name').setValue(row.data.brand_name);
            Ext.getCmp('brand_name').show();
        }

        //是否重複送
        switch (row.data.repeat) {
            case "false":
                Ext.getCmp("OnlyOne").setValue(true);
                break;
            case "true":
                Ext.getCmp("Every").setValue(true);
                break;
            default:
                Ext.getCmp("OnlyOne").setValue(true);
                break;
        }



    }

    function Tomorrow() {
        var d;
        var dt;
        var s = "";
        d = new Date();                             // 创建 Date 对象。
        s += d.getFullYear() + "/";                     // 获取年份。
        s += (d.getMonth() + 1) + "/";              // 获取月份。
        s += d.getDate();
        dt = new Date(s);
        dt.setDate(dt.getDate() + 1);
        return dt;                                 // 返回日期。
    }

}

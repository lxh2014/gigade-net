var promoDisID = "";
var produCateID = "";
var event_type = "";
var conditionID = "";
var boolClass;

Ext.define('gigade.PromoDiscount', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "rid", type: "int" },
        { name: "event_id", type: "string" },
        { name: "quantity", type: "int" },
        { name: "discount", type: "int" }
    ]
});


var PromoDisStore = Ext.create('Ext.data.Store', {
    storeId: 'PromoDisStore',
    model: 'gigade.PromoDiscount',
    proxy: {
        type: 'ajax',
        url: '/PromotionsDiscount/PromoDiscountList',
        reader: {
            type: 'json',
            root: 'data'
        }
    },
    listeners: {
        update: function (store, record) {
            if (record.isModified('quantity') || record.isModified('discount')) {
                Ext.Ajax.request({
                    url: '/PromotionsDiscount/SavePromoDis',
                    params: {
                        rid: Ext.htmlEncode(record.get("rid")),
                        id: Ext.htmlEncode(promoDisID),
                        eventType: Ext.htmlEncode(event_type),
                        jianshu: Ext.htmlEncode(record.get("quantity")),
                        discount: Ext.htmlEncode(record.get("discount"))
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            PromoDisStore.load({ params: { Proid: promoDisID, ProType: event_type } });
                        } else if (result.msg.toString() == "1") {
                            Ext.Msg.alert(INFORMATION, PROMODISCOUNTUPDATETIP);
                            PromoDisStore.load({ params: { Proid: promoDisID, ProType: event_type } });
                        } else {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        }
    }
});

function fewDiscountForm(rowID, PriceStore, condition_name, Type) {
    var linkPath = "http://www.gigade100.com/promotion/combo_promotion.php?event_id="; //保存图片链接的地址
    var websiteID = 1;
    var currentPanel = 0;
    var condiCount = 0; //商品類別條件設定點擊次數
    if (rowID != "") {
        promoDisID = rowID.data["id"];
        produCateID = rowID.data["category_id"];
        conditionID = rowID.data["condition_id"];
        websiteID = rowID.data.siteId == "" ? 1 : rowID.data.siteId;
    } else {
        promoDisID = "";
        produCateID = "";
        conditionID = "";
    }

    event_type = condition_name;

    function onAdd() {
        var jianshu = Ext.getCmp("jianshu");
        var discount = Ext.getCmp("is_discount");
        if (jianshu.isValid() && discount.isValid()) {
            Ext.Ajax.request({
                url: '/PromotionsDiscount/SavePromoDis',
                method: 'post',
                params: { id: promoDisID, eventType: event_type, jianshu: jianshu.getValue(), discount: discount.getValue() },
                success: function (form, action) {
                    var result = Ext.decode(form.responseText);
                    if (result.success) {
                        PromoDisStore.load({ params: { Proid: promoDisID, ProType: event_type } });
                    } else {
                        Ext.Msg.alert(INFORMATION, PROMODISCOUNTTIP);
                    }
                },
                failure: function () {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }
            });
        }
        else {
            return;
        }

    }

    var channelTpl = new Ext.XTemplate(
        '<a href="#">' + EDIT + '</a>');

    var rowEditing = Ext.create('Ext.grid.plugin.RowEditing', {
        clicksToMoveEditor: 1,
        listeners: {
            beforeedit: function (e, eOpts) {
                if (e.colIdx == 0) {
                    e.hide();
                }


            }
        },
        autoCancel: false,
        clicksToEdit: 1,
        errorSummary: false
    });
    Ext.grid.RowEditor.prototype.saveBtnText = SAVE;
    Ext.grid.RowEditor.prototype.cancelBtnText = CANCLE;

    var extGrid = new Ext.grid.Panel(
    {
        id: 'gdDis',
        store: PromoDisStore,
        height: 155,
        columnLines: true,
        plugins: [rowEditing],
        frame: true,
        columns: [
        {
            header: DELETE, xtype: 'actioncolumn', width: 40, align: 'center',
            items: [{
                icon: '../../../Content/img/icons/cross.gif',
                tooltip: '刪除',
                handler: function (grid, rowIndex, colIndex) {
                    //提示是否刪除
                    var rec = grid.getStore().getAt(rowIndex);
                    var rid = rec.get('rid');
                    Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, 1), function (btn) {
                        if (btn == 'yes') {
                            Ext.Ajax.request({
                                url: '/PromotionsDiscount/DeletePromoDis',
                                method: 'post',
                                params: { rid_del: rid },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        PromoDisStore.load({ params: { Proid: promoDisID, ProType: event_type } });
                                    }
                                },
                                failure: function () {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            });

                        }
                    });
                }
            }]
        },
                 { header: RID, dataIndex: 'rid', width: 80, align: 'center' },
            { header: EVENTID, dataIndex: 'event_id', width: 80, align: 'center' },
            {
                header: QUANTITY, dataIndex: 'quantity', width: 85, align: 'center', flex: 1,
                editor: {
                    xtype: "numberfield",
                    allowBlank: false,
                    minValue: 1
                }
            },
            {
                header: DISCOUNT, dataIndex: 'discount', width: 90, align: 'center', flex: 1,
                editor: {
                    xtype: "numberfield",
                    allowBlank: false,
                    minValue: 10,
                    maxValue: 100
                }
            },
            { header: EDIT, xtype: 'templatecolumn', tpl: channelTpl, align: 'center' }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });

    PromoDisStore.load({ params: { Proid: promoDisID, ProType: event_type } });
    if (!rowID) {
        SiteStore.load();
    }
    VipGroupStore.load();
    ShopClassStore.load();
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
           topValue: Ext.htmlEncode(Ext.getCmp("class_id").getValue())

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

    var navigate = function (panel, direction) {
        // 此程序可以包含一些控制导航步骤的必要业务逻辑. 比如调用setActiveItem, 管理导航按钮的状态,
        // 处理可能出现的分支逻辑, 处理特殊操作像取消或结束等等. 一个完整的向导页, 对于复杂的需求
        // 实现起来可能也会相当复杂, 在实际的程序中通常应该以继承CardLayout的方式来实现. 
        var layout = panel.getLayout();
        if ('next' == direction) {
            if (currentPanel == 0) {//首頁時進行第一步保存
                if (!rowID) {
                    if (promoDisID == "") {

                        var fform = firstForm.getForm();
                        if (fform.isValid()) {
                            Ext.Ajax.request({
                                url: '/PromotionsDiscount/SavePromotionsAmountDiscount',
                                method: 'post',
                                params: {
                                    name: Ext.getCmp('name').getValue(),
                                    desc: Ext.getCmp('event_desc').getValue(),
                                    e_type: Ext.htmlEncode(event_type),
                                    devicename: Ext.htmlEncode(Ext.getCmp("devicename").getValue().devicename),
                                    start_date: Ext.htmlEncode(Ext.getCmp('startdate').getRawValue()),
                                    end_date: Ext.htmlEncode(Ext.getCmp('enddate').getRawValue()),
                                    vendor_coverage: Ext.htmlEncode(Ext.getCmp('vendor_coverage').getValue()),
                                    site: Ext.htmlEncode(Ext.getCmp('site').getValue())
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        promoDisID = result.id;
                                        produCateID = result.cateID;
                                        var banner_image = Ext.getCmp("banner_url");
                                        banner_image.setValue(linkPath + GetEventId(event_type, promoDisID));

                                        layout[direction]();
                                        currentPanel++;
                                        if (!layout.getNext()) {

                                            Ext.getCmp('move-next').hide();
                                            Ext.getCmp('move-prev').setDisabled(false);
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
                    else {
                        var fform = firstForm.getForm();
                        if (fform.isValid()) {
                            var banner_image = Ext.getCmp("banner_url");
                            banner_image.setValue(linkPath + GetEventId(event_type, promoDisID));

                            layout[direction]();
                            currentPanel++;
                            if (!layout.getNext()) {
                                Ext.getCmp('move-next').hide();
                            }
                            else {
                                Ext.getCmp('move-next').setText(NEXT_MOVE);
                            }
                        }
                    }
                }
                else {
                    var fform = firstForm.getForm();
                    if (fform.isValid()) {
                        var banner_image = Ext.getCmp("banner_url");
                        if (rowID.data.category_link_url == "") {
                            banner_image.setValue(linkPath + GetEventId(event_type, promoDisID));
                        }
                        else {
                            banner_image.setValue(rowID.data.category_link_url);
                        }
                        layout[direction]();
                        currentPanel++;
                        if (!layout.getNext()) {

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
            }
        }
        Ext.getCmp('move-prev').setDisabled(!layout.getPrev());
    };
    var firstForm = Ext.widget('form',
           {
               id: 'editFrm1',
               frame: true,
               plain: true,
               defaultType: 'textfield',
               layout: 'anchor',
               defaults: { anchor: "95%", msgTarget: "side" },
               items: [
                {//活動名稱
                    xtype: "textfield",
                    fieldLabel: ACTIVENAME,
                    id: 'name',
                    name: 'name',
                    allowBlank: false,
                    submitValue: true

                },
                   {//活動描述
                       xtype: "textfield",
                       fieldLabel: ACTIVEDESC,
                       id: 'event_desc',
                       name: 'event_desc',
                       allowBlank: false,
                       submitValue: true
                   },
                      {
                          xtype: 'radiogroup',
                          hidden: false,
                          id: 'devicename',
                          name: 'devicename',
                          fieldLabel: DEVICE,
                          colName: 'devicename',
                          anchor: '100%',
                          defaults: {
                              name: 'devicename',
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
                            id: 'startdate',
                            name: 'start_date',
                            format: 'Y-m-d H:i:s',
                            width: 150,
                            allowBlank: false,
                            submitValue: true,
                            value: Tomorrow(),
                            time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                            listeners: {
                                select: function (a, b, c) {
                                    var start = Ext.getCmp("startdate");
                                    var end = Ext.getCmp("enddate");
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
                          id: 'enddate',
                          name: 'end_date',
                          format: 'Y-m-d H:i:s',
                          width: 150,
                          allowBlank: false,
                          submitValue: true, //
                          value: setNextMonth(Tomorrow(), 1),
                          listeners: {
                              select: function (a, b, c) {
                                  var start = Ext.getCmp("startdate");
                                  var end = Ext.getCmp("enddate");
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
    var secondForm = Ext.widget('form',
           {
               id: 'editFrm2',
               frame: true,
               plain: true,
               height: 420,
               autoScroll: true,
               defaultType: 'textfield',
               layout: 'anchor',
               url: '/PromotionsDiscount/SavePromotionsAmountDiscountTwo',
               defaults: { anchor: "95%", msgTarget: "side" },
               items: [
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
                         id: 'group_name',
                         name: 'group_name',
                         hiddenName: 'group_id',
                         lastQuery: '',
                         store: VipGroupStore,
                         displayField: 'group_name',
                         submitValue: true,
                         valueField: 'group_id',
                         typeAhead: true,
                         forceSelection: false,
                         value: "0"

                     }
                      ]
                  }, {
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
                     }
                      ]
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
                                // layout: 'hbox',
                                anchor: '100%',
                                // hidden: true,
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
                                               banner_url.setValue(linkPath + GetEventId(event_type, promoDisID));
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
                                           var photo = Ext.getCmp("photo");
                                           var isBannerPanel = Ext.getCmp("isBanner");
                                           var noBannerPanel = Ext.getCmp("noBanner");
                                           if (newValue) {
                                               isBannerPanel.hide();
                                               banner_url.setValue("0");
                                               banner_url.allowBlank = true;
                                               banner_url.setValue("");
                                               banner_url.isValid();
                                               photo.setValue("0");
                                               photo.allowBlank = true;
                                               photo.setValue("");
                                               photo.isValid();
                                               noBannerPanel.show();
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
                  fieldLabel: SHOPCLASS,
                  editable: false,
                  id: 'class_id',
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
                  listeners: {
                      "select": function (combo, record) {
                          var z = Ext.getCmp("brand_id");
                          z.clearValue();
                          classBrandStore.removeAll();
                          boolClass = true;
                          z.setDisabled(false);
                          if (combo.getValue() != "") {
                              z.allowBlank = false;
                          }
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
                 hiddenname: 'brand_id',
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
                                     topValue: Ext.htmlEncode(Ext.getCmp("class_id").getValue())
                                 }
                             });
                             boolClass = false;
                         }
                     },
                     'blur': function () {
                         var o = classBrandStore.data.items;
                         var beand_name_frank = '';

                         if (document.getElementsByName('brand_name')[0].value != Ext.getCmp('brand_id').getValue()) {
                             document.getElementsByName('brand_name')[0].value = classBrandStore.getAt(0).get('brand_id');
                         }
                     }
                 }
             },
               //促銷商品類別
            {
                xtype: 'form', //類別設定
                layout: 'hbox',
                margin: '0 0 5 0',
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
                                  sclass.setValue("");
                                  brand.setValue("");
                                  brand.setDisabled(true);
                                  condiCount++;
                                  var regex = /^([0-9]+,)*[0-9]+$/; //類似1|1，3，5|12,4
                                  if (!regex.test(websiteID)) {
                                      websiteID = rowID.data.siteId;
                                  }
                                  PromationMationShow(produCateID, websiteID);
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
                       xtype: 'fieldcontainer',
                       defaults: {
                           labelWidth: 50,
                           width: 120,
                           margin: '0 10 0 0'
                       },
                       id: 'ls',
                       combineErrors: true,
                       layout: 'hbox',
                       items: [
                                {
                                    xtype: "numberfield",
                                    fieldLabel: JIANSHU,
                                    allowBlank: false,
                                    id: 'jianshu',
                                    name: 'jianshu',
                                    minValue: 0,
                                    value: 0
                                },
                                {
                                    xtype: "numberfield",
                                    fieldLabel: DISCOUNT,
                                    allowBlank: false,
                                    id: 'is_discount',
                                    name: 'is_discount',
                                    minValue: 10,
                                    value: 10,
                                    labelWidth: 60,
                                    maxValue: 100,
                                    submitValue: true,
                                    listeners: {
                                        change: function (field, newValue, oldValue) {

                                            if (newValue !== oldValue) {//网上银行    
                                                Ext.getCmp('is_discount').getEl().first().dom.innerHTML = DISCOUNT + ":<font style='color:red'>" + (100 - newValue) + "%</font>";
                                                //label = Ext.getCmp('no_discount').getEl().parent().parent().first();
                                                //  label.dom.innerHTML = DISCOUNT+(100-newValue)+"%";
                                                //Ext.DomQuery.selectNode('label[for=no_discount]').innerHTML =DISCOUNT+(100-newValue)+ '%';
                                            }
                                        }
                                    }
                                }, {
                                    xtype: 'button',
                                    text: ADD,
                                    id: 'p_add',
                                    width: 60,
                                    handler: onAdd

                                }]
                   },
                    {
                        xtype: 'fieldcontainer',
                        defaults: {
                            margin: '0 10 0 0'
                        },
                        items: [
                               {
                                   xtype: "displayfield",
                                   value: "<span style='font-size:12px; color:#F00;'>折扣範例 : 8折請填80; 9折請填90 </span>"
                               }
                        ]
                    },
                    extGrid
                       ]
                   },
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
                  emptyText: "不分",
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
                                 if (!rowID) {
                                     Ext.getCmp("no_brand_id").setValue(classBrandStore_no.data.items[0].data.brand_id);
                                 }
                                 else {

                                     Ext.getCmp("no_brand_id").setValue(rowID.data.brand_id);
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
                                id: 'noall',
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
                                        Ext.getCmp('no_allClass').setValue(false);
                                    }
                                }
                            },
                             {
                                 xtype: 'checkbox',
                                 boxLabel: ALLSHOP,
                                 id: 'no_allClass',
                                 name: 'allClass',
                                 margin: '0 0 0 20',
                                 inputValue: '1',
                                 listeners: {
                                     change: function (checkbox, newValue, oldValue) {
                                         var sclass = Ext.getCmp('no_class_id');
                                         var brand = Ext.getCmp('no_brand_id');
                                         var product = Ext.getCmp('product_id');
                                         if (newValue) {
                                             sclass.setValue(0);
                                             brand.setValue(0);
                                             product.setValue(999999);
                                             Ext.getCmp('product_name').setValue("全館商品");
                                         }
                                         else {
                                             product.setValue(0);
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
                            },
                            {
                                xtype: "numberfield",
                                fieldLabel: ALLMONEY,
                                id: 'no_amount',
                                name: 'amount',
                                allowBlank: true,
                                allowDecimals: false, // 允许小数点
                                allowNegative: false, // 允许负数
                                minValue: 0,
                                value: 0,
                                submitValue: true
                            },
                            {
                                xtype: "numberfield",
                                fieldLabel: FAREQUANTITY,
                                id: 'no_quantity',
                                name: 'quantity',
                                allowBlank: true,
                                allowDecimals: false, // 允许小数点
                                allowNegative: false, // 允许负数
                                minValue: 0,
                                value: 0,
                                submitValue: true
                            },
                             {
                                 xtype: "numberfield",
                                 fieldLabel: DISCOUNT,
                                 allowDecimals: false, // 允许小数点
                                 allowNegative: false, // 允许负数
                                 id: 'no_discount',
                                 name: 'discount',
                                 allowBlank: true,
                                 minValue: 0,
                                 value: 0,
                                 submitValue: true,
                                 listeners: {
                                     change: function (field, newValue, oldValue) {

                                         if (newValue !== oldValue) {//网上银行   
                                             Ext.getCmp('no_discount').getEl().first().dom.innerHTML = DISCOUNT + ":<font style='color:red'>" + newValue + "%</font>";
                                         }
                                     }
                                 }
                             },

                                        {
                                            xtype: "displayfield",
                                            height: 50,
                                            value: "<span style='font-size:12px; color:#F00;'>折扣範例 : 8折請填20; 9折請填10 </span>"
                                        }



                   ]
               }
               ],
               buttons: [{
                   text: SAVE,
                   formBind: true,
                   disabled: false,
                   handler: function () {
                     
                       var form = this.up('form').getForm();
                       if (form.isValid()) {
                           if (!Ext.getCmp('allClass').getValue() && (condiCount == 0 && rowID == null) && (Ext.getCmp('brand_id').getValue() == "" || Ext.getCmp('brand_id').getValue() == null || Ext.getCmp('brand_id').getValue() == "0")) {
                               Ext.Msg.alert(INFORMATION, CONDITIONTIP);
                               return;
                           }

                           if (condition_id == 0 && (Ext.getCmp("group_name").getValue() == "" || Ext.getCmp("group_name").getValue() == null)) {
                               Ext.Msg.alert(INFORMATION, USERCONDITIONERROR);
                               return;
                           }
                           this.disable();
                           form.submit({

                               params: {
                                   type: Ext.htmlEncode(Type),
                                   pid: Ext.htmlEncode(promoDisID),
                                   name: Ext.getCmp('name').getValue(),
                                   desc: Ext.getCmp('event_desc').getValue(),
                                   e_type: Ext.htmlEncode(event_type),
                                   cateId: Ext.htmlEncode(produCateID),
                                   //   url_by: Ext.htmlEncode(Ext.getCmp("url_by").getValue().url_by),
                                   banner_url: Ext.htmlEncode(Ext.getCmp("banner_url").getValue()),
                                   sbrand_id: document.getElementsByName('brand_name').length == 0 ? '' : Ext.htmlEncode(document.getElementsByName('brand_name')[0].value),
                                   sclass_id: Ext.htmlEncode(Ext.getCmp('class_id').getValue()),
                                   group_id: Ext.htmlEncode(Ext.getCmp('group_name').getValue()),
                                   condition_id: Ext.htmlEncode(condition_id),
                                   devicename: Ext.htmlEncode(Ext.getCmp("devicename").getValue().devicename),
                                   start_date: Ext.htmlEncode(Ext.getCmp('startdate').getRawValue()),
                                   end_date: Ext.htmlEncode(Ext.getCmp('enddate').getRawValue()),
                                   vendor_coverage: Ext.htmlEncode(Ext.getCmp('vendor_coverage').getValue()),
                                   allClass: Ext.htmlEncode(Ext.getCmp('allClass').getValue()),
                                   site: Ext.htmlEncode(Ext.getCmp('site').getValue())
                                   ,
                                   noclass_id: Ext.htmlEncode(Ext.getCmp('no_class_id').getValue()),
                                   nobrand_id: Ext.htmlEncode(Ext.getCmp('no_brand_id').getValue()),
                                   product_id: Ext.htmlEncode(Ext.getCmp('product_id').getValue()),
                                   no_amount: Ext.htmlEncode(Ext.getCmp('no_amount').getValue()),
                                   no_quantity: Ext.htmlEncode(Ext.getCmp('no_quantity').getValue()),
                                   no_discount: Ext.htmlEncode(Ext.getCmp('no_discount').getValue()),
                                   no_allClass: Ext.htmlEncode(Ext.getCmp('no_allClass').getValue())
                               },
                               success: function (form, action) {
                                   var result = Ext.decode(action.response.responseText);
                                   if (result.success) {
                                       Ext.Msg.alert(INFORMATION, SUCCESS);
                                       editWin.close();
                                       PriceStore.load();

                                   }
                                   else {

                                       Ext.Msg.alert(INFORMATION, FAILURE);
                                   }
                               },
                               failure: function (form, action) {
                                   var result = Ext.decode(action.response.responseText);
                                   if (result.msg == "1") {
                                       Ext.Msg.alert(INFORMATION, PHOTOSIZE);
                                   }
                                   else if (result.msg == "2") {
                                       Ext.Msg.alert(INFORMATION, PHOTOTYPE);
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
        url: '/PromotionsDiscount/SavePromotionsAmountDiscountTwo',
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
        items: [firstForm, secondForm]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: FEWPIECEFEWDISCOUNT,
        iconCls: rowID ? "icon-user-edit" : "icon-user-add",
        width: 400,
        y: 100,
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
                if (rowID) {
                    firstForm.getForm().loadRecord(rowID);
                    secondForm.getForm().loadRecord(rowID);
                    initForm(rowID);
                }
                else {
                    classBrandStore_no.load({
                        callback: function () {
                            Ext.getCmp("no_brand_id").setValue(0);
                        }
                    });
                    firstForm.getForm().reset();
                    secondForm.getForm().reset();
                }
            }
        }
    });
    editWin.show();
    function initForm(rowID) {

        if (rowID != "") {
            if (rowID.data.url_by == 1) {//是專區
                Ext.getCmp("bt1").setValue(true);
                var img = rowID.data.banner_image.toString();
                var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
                Ext.getCmp('photo').setRawValue(imgUrl);

                if ("1" == rowID.data.isallclass) {
                    Ext.getCmp('allClass').setValue(true);
                }
                else {
                    Ext.getCmp('allClass').setValue(false);
                }
                Ext.getCmp('brand_id').setRawValue(rowID.data.brand_name);
                if (rowID.data.class_id == "0") {
                    Ext.getCmp('class_id').setValue("");
                }
                if (rowID.data.class_id != 0 && rowID.data.brand_id != 0) {
                    Ext.getCmp('brand_id').setDisabled(false);
                    classBrandStore.load({
                        params: {
                            topValue: Ext.getCmp("class_id").getValue()
                        }
                    });
                }
                //清空非專區
                Ext.getCmp('no_class_id').setValue(0);
                Ext.getCmp('no_brand_id').setValue(0);
                Ext.getCmp('product_id').setValue(0);
                Ext.getCmp('no_amount').setValue(0);
                Ext.getCmp('no_discount').setValue(0);
                Ext.getCmp('no_quantity').setValue(0);
            }
            else {//非專區
                //清空專區
                Ext.getCmp("bt2").setValue(true);
                Ext.getCmp("banner_url").setValue("");
                //賦值非專區
                if (rowID.data.class_id == 0 || rowID.data.class_id == "" || rowID.data.class_id == "0") {
                    Ext.getCmp('no_class_id').setValue(0);
                }
                else {
                    Ext.getCmp('no_class_id').setValue(rowID.data.class_id);
                }

                classBrandStore_no.load({
                    params: {
                        topValue: rowID.data.class_id
                    },
                    callback: function () {
                        Ext.getCmp("no_brand_id").setValue(rowID.data.brand_id);
                    }
                });

                if (rowID.data.product_id != 0) {
                    Ext.Ajax.request({
                        url: '/PromotionsDiscount/GetProdName',
                        params: {
                            product_id: Ext.htmlEncode(rowID.data.product_id),
                            class_id: Ext.htmlEncode(rowID.data.class_id),
                            brand_id: Ext.htmlEncode(rowID.data.brand_id)
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

                //  Ext.getCmp('no_brand_id').setValue(rowID.data.brand_id);
                Ext.getCmp('product_id').setValue(rowID.data.product_id);

                Ext.getCmp('no_amount').setValue(rowID.data.amount);
                Ext.getCmp('no_discount').setValue(rowID.data.discount);
                Ext.getCmp('no_quantity').setValue(rowID.data.quantity);
            }
            if (rowID.data.condition_id != 0) {

                Ext.getCmp('us1').setValue(false);
                Ext.getCmp('us2').setValue(true);
            }
            else {
                Ext.getCmp('us1').setValue(true);
                Ext.getCmp('us2').setValue(false);
                if (rowID.data.group_name == "") {
                    Ext.getCmp('group_name').setValue("0");
                }
            }


            switch (rowID.data.devicename) {
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

            //修改時對site賦值
            SiteStore.load({
                callback: function () {
                    var siteIDs = rowID.data.siteId.toString().split(',');
                    var arrTemp = new Array();
                    for (var i = 0; i < siteIDs.length; i++) {
                        arrTemp.push(SiteStore.getAt(SiteStore.find("Site_Id", siteIDs[i])));
                    }
                    Ext.getCmp('site').setValue(arrTemp);

                }
            })



        }
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


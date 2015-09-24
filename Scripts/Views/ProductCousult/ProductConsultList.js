var pageSize = 20;
var boolPassword = true;//secretcopy
var info_type = "users";
var secret_info = "user_name";
/*******************群組管理主頁面****************************/
//自定义VTypes类型，验证日期范围  
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
//群組管理Model
Ext.define('gigade.ProductConsultModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "consult_id", type: "int" },
        { name: "product_name", type: "string" },
        { name: "user_email", type: "string" },
        { name: "product_id", type: "int" },
        { name: "user_id", type: "int" },
        { name: "consult_info", type: "string" },
        { name: "status", type: "int" },
        { name: "consult_answer", type: "string" },
        { name: "consult_type", type: "int" },
        { name: "parameterName", type: "string" },
        { name: "user_name", type: "string" },
        { name: "is_sendEmail", type: "int" },
        { name: "answer_user", type: "int" },
        { name: "create_date", type: "string" },
        { name: "answer_date", type: "string" },
        { name: "manage_name", type: "string" },
         { name: "product_url", type: "string" },
        { name: "consult_url", type: "string" },
        { name: "item_id", type: "int" },
        { name: "answer_status", type: "int" },
        { name: "delay_reason", type: "string" },
        { name: "prod_classify", type: "string" }
    ]
});

Ext.define('gigade.ParameterModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "parameterName", type: "string" },
        { name: "parameterCode", type: "string" }
    ]
});

var ProductConsultStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.ProductConsultModel',
    proxy: {
        type: 'ajax',
        url: '/ProductConsult/GetProductConsultList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
var edit_ProductConsultStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.ProductConsultModel',
    proxy: {
        type: 'ajax',
        url: '/ProductConsult/GetProductConsultList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

//前面選擇框 選擇之後顯示編輯刪除
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("pcGift").down('#edit').setDisabled(selections.length == 0);
            //  Ext.getCmp("pcGift").down('#remove').setDisabled(selections.length == 0);
        }
    }
});

//定義ddl的數據
var ParameterStore = Ext.create('Ext.data.Store', {
    fields: ['parameterName', 'parameterCode'],
    data: [
        { "parameterName": '食品館', "parameterCode": "10" },
        { "parameterName": '用品館', "parameterCode": "20" }
    ]
});

//加載前先獲取ddl的值
ProductConsultStore.on('beforeload', function () {
    Ext.apply(ProductConsultStore.proxy.extraParams, {
        ddlSel: Ext.getCmp('ddlSel').getValue(),
        productName: Ext.getCmp('productName').getValue(),
        //shopClass: Ext.getCmp('shopClass').getValue(),
        timestart: Ext.getCmp('timestart').getValue(),
        timeend: Ext.getCmp('timeend').getValue(),
        productId: Ext.getCmp('productId').getValue(),
        //consultType: Ext.getCmp('consultType').getValue(),
        userName: Ext.getCmp('userName').getValue(),
        userEmail: Ext.getCmp('userEmail').getValue(),
        shopClass1: Ext.getCmp('ckShopClass1').getValue(),
        shopClass2: Ext.getCmp('ckShopClass2').getValue(),
        consultType1: Ext.getCmp('zixunType1').getValue(),
        consultType2: Ext.getCmp('zixunType2').getValue(),
        consultType3: Ext.getCmp('zixunType3').getValue(),
        consultType4: Ext.getCmp('zixunType4').getValue(),
        consultType5: Ext.getCmp('zixunType5').getValue(),
        relation_id: "",
        isSecret: true
    });
});




//加載前先獲取ddl的值
edit_ProductConsultStore.on('beforeload', function () {
    Ext.apply(edit_ProductConsultStore.proxy.extraParams, {
        ddlSel: Ext.getCmp('ddlSel').getValue(),
        productName: Ext.getCmp('productName').getValue(),
        //shopClass: Ext.getCmp('shopClass').getValue(),
        timestart: Ext.getCmp('timestart').getValue(),
        timeend: Ext.getCmp('timeend').getValue(),
        productId: Ext.getCmp('productId').getValue(),
        //consultType: Ext.getCmp('consultType').getValue(),
        userName: Ext.getCmp('userName').getValue(),
        userEmail: Ext.getCmp('userEmail').getValue(),
        shopClass1: Ext.getCmp('ckShopClass1').getValue(),
        shopClass2: Ext.getCmp('ckShopClass2').getValue(),
        consultType1: Ext.getCmp('zixunType1').getValue(),
        consultType2: Ext.getCmp('zixunType2').getValue(),
        consultType3: Ext.getCmp('zixunType3').getValue(),
        consultType4: Ext.getCmp('zixunType4').getValue(),
        consultType5: Ext.getCmp('zixunType5').getValue(),
        relation_id: "",
        isSecret: false

    });
});

var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '所有狀態', "value": "0" },
        { "txt": '待回覆', "value": "1" },
        { "txt": '處理中', "value": "2" },
        { "txt": '已回覆', "value": "3" }
    ]
});
var HFStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '所有狀態', "value": "0" },
        { "txt": '回覆顯示', "value": "1" },
        { "txt": '回覆不顯示', "value": "2" }
    ]
});
var consultTypeStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '商品諮詢', "value": "1" },
        { "txt": '庫存及配送', "value": "2" },
        { "txt": '支付問題', "value": "3" },
        { "txt": '發票及保修', "value": "4" },
        { "txt": '促銷及贈品', "value": "5" }
    ]
});
function Query(x) {
    Ext.getCmp("pcGift").show();
    ProductConsultStore.removeAll();
    var SPG = Ext.getCmp('ckShopClass1').getValue();
    var YPG = Ext.getCmp('ckShopClass2').getValue();
    var SPZX = Ext.getCmp('zixunType1').getValue();
    var KCPS = Ext.getCmp('zixunType2').getValue();
    var ZFWT = Ext.getCmp('zixunType3').getValue();
    var FPBX = Ext.getCmp('zixunType4').getValue();
    var CXZP = Ext.getCmp('zixunType5').getValue();
    if (!SPG && !YPG) {
        Ext.Msg.alert("提示信息", "請選擇館別");
        return;
    }
    if (!SPZX && !KCPS && !ZFWT && !FPBX && !CXZP) {
        Ext.Msg.alert("提示信息", "請選擇諮詢類型");
        return;
    }
    Ext.getCmp("pcGift").store.loadPage(1, {
        params: {
            ddlSel: Ext.getCmp('ddlSel').getValue(),
            huifu: Ext.getCmp('huifu').getValue(),
            //shopClass: Ext.getCmp('shopClass').getValue(),
            productName: Ext.getCmp('productName').getValue(),
            timestart: Ext.getCmp('timestart').getValue(),
            timeend: Ext.getCmp('timeend').getValue(),
            productId: Ext.getCmp('productId').getValue(),
            //consultType: Ext.getCmp('consultType').getValue(),
            userName: Ext.getCmp('userName').getValue(),
            userEmail: Ext.getCmp('userEmail').getValue(),
            shopClass1: SPG,
            shopClass2: YPG,
            consultType1: SPZX,
            consultType2: KCPS,
            consultType3: ZFWT,
            consultType4: FPBX,
            consultType5: CXZP
        }
    });
}
//加載前先獲取ddl的值
ProductConsultStore.on('beforeload', function () {
    ProductConsultStore.removeAll();
    var SPG = Ext.getCmp('ckShopClass1').getValue();
    var YPG = Ext.getCmp('ckShopClass2').getValue();
    var SPZX = Ext.getCmp('zixunType1').getValue();
    var KCPS = Ext.getCmp('zixunType2').getValue();
    var ZFWT = Ext.getCmp('zixunType3').getValue();
    var FPBX = Ext.getCmp('zixunType4').getValue();
    var CXZP = Ext.getCmp('zixunType5').getValue();
    if (!SPG && !YPG) {
        Ext.Msg.alert("提示信息", "請選擇館別");
        return;
    }
    if (!SPZX && !KCPS && !ZFWT && !FPBX && !CXZP) {
        Ext.Msg.alert("提示信息", "請選擇諮詢類型");
        return;
    }
    Ext.apply(ProductConsultStore.proxy.extraParams, {
        ddlSel: Ext.getCmp('ddlSel').getValue(),
        huifu: Ext.getCmp('huifu').getValue(),
        //shopClass: Ext.getCmp('shopClass').getValue(),
        productName: Ext.getCmp('productName').getValue(),
        timestart: Ext.getCmp('timestart').getValue(),
        timeend: Ext.getCmp('timeend').getValue(),
        productId: Ext.getCmp('productId').getValue(),
        //consultType: Ext.getCmp('consultType').getValue(),
        userName: Ext.getCmp('userName').getValue(),
        userEmail: Ext.getCmp('userEmail').getValue(),
        shopClass1: SPG,
        shopClass2: YPG,
        consultType1: SPZX,
        consultType2: KCPS,
        consultType3: ZFWT,
        consultType4: FPBX,
        consultType5: CXZP
    });
});
//頁面載入
Ext.onReady(function () {
    var searchForm = Ext.create('Ext.form.Panel', {
        id: 'searchForm',
        layout: 'anchor',
        title: '查詢條件',
        height: 150,
        border: 0,
        bodyPadding: 10,
        flex: 3,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                     {
                         xtype: 'textfield',
                         allowBlank: true,
                         id: 'productName',
                         //padding: "0 0 5 0",
                         margin: "0 5 0 0",
                         name: 'productName',
                         fieldLabel: '商品名稱',
                         labelWidth: 60,
                         listeners: {
                             specialkey: function (field, e) {
                                 if (e.getKey() == e.ENTER) {
                                     Query();
                                 }
                             }
                         }
                     },
                     {
                         xtype: 'textfield',
                         allowBlank: true,
                         id: 'productId',
                         margin: "0 5 0 0",
                         name: 'productId',
                         fieldLabel: '商品編號',
                         labelWidth: 60,
                         listeners: {
                             specialkey: function (field, e) {
                                 if (e.getKey() == e.ENTER) {
                                     Query();
                                 }
                             }
                         }
                     },
             {
                 xtype: 'textfield',
                 allowBlank: true,
                 id: 'userName',
                 name: 'userName',
                 margin: "0 5 0 0",
                 fieldLabel: '用戶名稱',
                 labelWidth: 60,
                 listeners: {
                     specialkey: function (field, e) {
                         if (e.getKey() == e.ENTER) {
                             Query();
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
                 xtype: 'textfield',
                 allowBlank: true,
                 id: 'userEmail',
                 name: 'userEmail',
                 margin: "0 5 0 0",
                 fieldLabel: '用戶郵箱',
                 labelWidth: 60,
                 listeners: {
                     specialkey: function (field, e) {
                         if (e.getKey() == e.ENTER) {
                             Query();
                         }
                     }
                 }
             },
              {
                  xtype: 'combobox',
                  editable: false,
                  fieldLabel: '回覆狀態',
                  labelWidth: 60,
                  margin: "0 5 0 0",
                  id: 'ddlSel',
                  store: DDLStore,
                  displayField: 'txt',
                  valueField: 'value',
                  value: 1,
                  //emptyValue: '所有狀態',
                  labelWidth: 60
              },
              {
                  xtype: 'combobox',
                  editable: false,
                  fieldLabel: '顯示狀態',
                  labelWidth: 60,
                  margin: "0 5 0 0",
                  id: 'huifu',
                  store: HFStore,
                  displayField: 'txt',
                  valueField: 'value',
                  value: 0,
                  //emptyValue: '所有狀態',
                  labelWidth: 60
              }

                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'fieldset',
                        title: "館別",
                        //defaultType: 'textfield',
                        id: 'userShow',
                        //hidden: true,
                        layout: 'hbox',
                        width: 215,
                        margin: "0 5 0 0",
                        items: [
                               {//顯示保存的條件設定
                                   xtype: 'checkbox',
                                   name: 'ckShopClass1',
                                   boxLabel: "食品館",
                                   id: 'ckShopClass1',
                                   hidden: true
                               }
                               ,
                               {//顯示保存的條件設定
                                   xtype: 'checkbox',
                                   name: 'ckShopClass2',
                                   boxLabel: "用品館",
                                   id: 'ckShopClass2',
                                   hidden: true
                               }
                        ]
                    },
                    {
                        xtype: 'fieldset',
                        title: "諮詢時間",
                        //defaultType: 'textfield',
                        id: 'timeShow',
                        //hidden: true,
                        layout: 'hbox',
                        width: 445,
                        margin: "0 5 0 0",
                        items: [
                               {
                                   xtype: 'datefield',
                                   allowBlank: true,
                                   id: 'timestart',
                                   margin: "0 5 0 0",
                                   name: 'serchcontent',
                                   fieldLabel: '諮詢時間',
                                   labelWidth: 60,
                                   editable: false,
                                   listeners: {
                                       select: function (a, b, c) {
                                           var tstart = Ext.getCmp("timestart");
                                           var tend = Ext.getCmp("timeend");
                                           if (tend.getValue() == null) {
                                               tend.setValue(setNextMonth(tstart.getValue(), 1));
                                           }
                                           else if (tend.getValue() < tstart.getValue()) {
                                               Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                                               tend.setValue(setNextMonth(tstart.getValue(), 1));
                                           }
                                       }
                                   }
                               },
                               {
                                   xtype: 'datefield',
                                   allowBlank: true,
                                   id: 'timeend',
                                   margin: "0 5 0 0",
                                   name: 'serchcontent',
                                   fieldLabel: '到',
                                   labelWidth: 15,
                                   editable: false,
                                   listeners: {
                                       select: function (a, b, c) {
                                           var tstart = Ext.getCmp("timestart");
                                           var tend = Ext.getCmp("timeend");
                                           if (tstart.getValue() == null) {
                                               tstart.setValue(setNextMonth(tend.getValue(), -1));
                                           }
                                           else if (tend.getValue() < tstart.getValue()) {
                                               Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                                               tstart.setValue(setNextMonth(tend.getValue(), -1));
                                           }
                                       }
                                   }
                               },
                        ]
                    }, ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'fieldset',
                        title: "諮詢類型",
                        //defaultType: 'textfield',
                        id: 'zxTypeShow',
                        //hidden: true,
                        layout: 'hbox',
                        width: 440,
                        margin: "0 5 50 0",
                        items: [
                      {//顯示保存的條件設定
                          xtype: 'checkbox',
                          name: 'zixunType1',
                          boxLabel: "商品諮詢",
                          id: 'zixunType1',
                          hidden: true
                      },
                    {//顯示保存的條件設定
                        xtype: 'checkbox',
                        name: 'zixunType2',
                        boxLabel: "庫存及配送",
                        //hidden: true,
                        id: 'zixunType2',
                        hidden: true
                    },
                    {//顯示保存的條件設定
                        xtype: 'checkbox',
                        name: 'zixunType3',
                        boxLabel: "支付問題",
                        //hidden: true,
                        id: 'zixunType3',
                        hidden: true
                    },
                    {//顯示保存的條件設定
                        xtype: 'checkbox',
                        name: 'zixunType4',
                        boxLabel: "發票及保修",
                        //hidden: true,
                        id: 'zixunType4',
                        hidden: true
                    },
                    {//顯示保存的條件設定
                        xtype: 'checkbox',
                        name: 'zixunType5',
                        boxLabel: "促銷及贈品",
                        //hidden: true,
                        id: 'zixunType5',
                        hidden: true
                    }
                        ]
                    },
                      {
                          xtype: 'button',
                          text: SEARCH,
                          iconCls: 'icon-search',
                          id: 'btnQuery',
                          width: 100,
                          height: 25,
                          margin: "5 0 0 0",
                          handler: Query
                      },
                    {
                        xtype: 'button',
                        text: RESET,
                        width: 100,
                        height: 25,
                        margin: "5 0 0 5",
                        iconCls: 'ui-icon ui-icon-reset',
                        id: 'btn_reset',
                        listeners: {
                            click: function () {
                                Ext.getCmp("productName").setValue("");
                                Ext.getCmp("ddlSel").reset();
                                Ext.getCmp("huifu").reset();
                                Ext.getCmp("timestart").setValue("");
                                Ext.getCmp("timeend").setValue("");
                                //Ext.getCmp("shopClass").setValue(null);
                                Ext.getCmp("productId").setValue("");
                                Ext.getCmp("userName").setValue("");
                                Ext.getCmp("userEmail").setValue("");
                                //Ext.getCmp("consultType").setValue(null);
                                Ext.getCmp("ckShopClass1").setValue(false);
                                Ext.getCmp("ckShopClass2").setValue(false);
                                Ext.getCmp("zixunType1").setValue(false);
                                Ext.getCmp("zixunType2").setValue(false);
                                Ext.getCmp("zixunType3").setValue(false);
                                Ext.getCmp("zixunType4").setValue(false);
                                Ext.getCmp("zixunType5").setValue(false);
                                Ext.getCmp("pcGift").hide();
                            }
                        }
                    }
                ]
            }
        ]
    });

    var pcGift = Ext.create('Ext.grid.Panel', {
        id: 'pcGift',
        store: ProductConsultStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        hidden: true,
        flex: 7,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
            { header: '編號', dataIndex: 'consult_id', width: 70, align: 'center', align: 'center', hidden: true },
            {
                header: '商品編號', dataIndex: 'product_id', width: 70, align: 'center'
            },
             { header: '子商品編號', dataIndex: 'item_id', width: 70, align: 'center', align: 'center', hidden: true },
            {
                header: '商品名稱', dataIndex: 'product_name', width: 200, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<a href='" + record.data.product_url + "' target='_blank'>" + value + "</a>";
                }
            },
            {
                header: '商品館別', dataIndex: 'prod_classify', width: 5, align: 'center', hidden: true
            },
             {
                 header: '用戶郵箱', dataIndex: 'user_email', width: 100, align: 'center', //hidden: true
             },

              {
                  header: '用戶名稱', dataIndex: 'user_name', width: 100, align: 'center',
                  renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                      return "<span onclick='SecretLogin(" + record.data.consult_id + "," + record.data.user_id + ",\"" + info_type + "\")'  >" + value + "</span>";

                  }
              },
              {
                  header: '諮詢類型', dataIndex: 'parameterName', width: 100, align: 'center'
              },
              //{ name: "consult_type", type: "int" },
              {
                  header: '諮詢類型編號', dataIndex: 'consult_type', width: 10, align: 'center', hidden: true
              },
               {
                   header: '諮詢內容', dataIndex: 'consult_info', width: 200, align: 'center'
               },
               {
                   header: '回覆狀態', dataIndex: 'answer_status', width: 100, align: 'center',
                   renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                       if (value == 1) {
                           return Ext.String.format('待回覆');
                       }
                       else if (value == 2) {
                           return "<span style='color:red'>處理中</span>";
                       }
                       else if (value == 3) {
                           return Ext.String.format('已回覆');
                       }

                   }
               },
               {
                   header: '推遲原因', dataIndex: 'delay_reason', width: 100, align: 'center',
               },
                {
                    header: '諮詢回覆', dataIndex: 'consult_answer', width: 200, align: 'center'
                },

            {
                header: "發送郵件", dataIndex: 'is_sendEmail', width: 70, align: 'center',
                renderer: function (value) {
                    if (value == 1) {
                        return Ext.String.format('是');
                    }
                    else if (value == 0) {
                        return Ext.String.format('否');
                    }
                }
            },
            //{ header: "詳情鏈接", dataIndex: 'consult_url', width: 100, align: 'center', align: 'center' },
            { header: "商品鏈接", dataIndex: 'product_url', width: 100, align: 'center', align: 'center', hidden: true },
             {
                 header: "回覆人", dataIndex: 'manage_name', width: 100, align: 'center', align: 'center',
             },
            { header: "諮詢時間", dataIndex: 'create_date', width: 150, align: 'center', align: 'center' },
            {
                header: "回覆時間", dataIndex: 'answer_date', width: 150, align: 'center', align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "0001-01-01 00:00:00") {
                        return null;
                    } else {
                        return value;
                    }
                }
            },
             {
                 header: '狀態',
                 dataIndex: 'status',
                 hidden: false,
                 align: 'center',
                 id: 'status',
                 hidden: false,
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (value == 1) {
                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.consult_id + ")'><img hidValue='0' id='img" + record.data.consult_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                     } else {
                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.consult_id + ")'><img hidValue='1' id='img" + record.data.consult_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                     }
                 }
             }
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: true, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: '回覆諮詢', id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            '->'
            //{
            //    text: SEARCH,
            //    iconCls: 'icon-search',
            //    id: 'btnQuery',
            //    handler: Query
            //},
            //{
            //    text: RESET,
            //    id: 'btn_reset',
            //    listeners: {
            //        click: function () {
            //            Ext.getCmp("ddlSel").setValue(null);
            //        }
            //    }
            //}
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ProductConsultStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [searchForm, pcGift],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                pcGift.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // ProductConsultStore.load({ params: { start: 0, limit: 25 } });
});


function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "6";//
    var url = "/ProductCousult/Index";
    var ralated_id = rid;
    //點擊機敏信息先保存記錄在驗證密碼是否需要輸入
    boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
    if (boolPassword != "-1") {//不准查看
        if (boolPassword) {//超過5分鐘沒有輸入密碼
            //參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息6.驗證通過后是否打開編輯窗口 7:客戶信息類型user:會員 order：訂單 vendor：供應商 8：客戶id9：要顯示的客戶信息
            //  function SecretLoginFun(type, relatedID, isLogin, isShow, editO, isEdit) {
            SecretLoginFun(secret_type, ralated_id, true, true, false, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框

        } else {
            SecretLoginFun(secret_type, ralated_id, false, true, false, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
        }
    }
}


//添加
onAddClick = function () {
    editFunction(null, ProductConsultStore);
}
//修改
onEditClick = function () {
    var row = Ext.getCmp("pcGift").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        var secret_type = "6";//參數表中的"會員查詢列表"
        var url = "/ProductCousult/Index/Edit ";
        var ralated_id = row[0].data.consult_id;
        var info_id = row[0].data.user_id;
        boolPassword = SaveSecretLog(url, secret_type, ralated_id);//判斷5分鐘之內是否有輸入密碼
        if (boolPassword != "-1") {
            if (boolPassword) {//驗證
                SecretLoginFun(secret_type, ralated_id, true, false, true, url, info_type, info_id, secret_info);//先彈出驗證框，關閉時在彈出顯示框
            } else {
                editFunction(ralated_id, ProductConsultStore);
            }
        }
        //editFunction(row[0], ProductConsultStore);
    }
}


//更改活動狀態(設置活動可用與不可用)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/ProductConsult/UpdateActive",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            ProductConsultStore.remove();
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                ProductConsultStore.load();
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                ProductConsultStore.load();
            }
        },
        error: function (msg) {
            Ext.Msg.alert('錯誤信息', '更改失敗');
        }
    });
}
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
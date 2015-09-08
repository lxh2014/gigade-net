var pageSize = 25;
var boolPassword = true;//secretcopy
var info_type = "users";
var secret_info = "";
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
Ext.define('gigade.ProductCommentModel', {
    extend: 'Ext.data.Model',
    fields: [
        
        { name: "comment_detail_id", type: "int" },
        { name: "comment_id", type: "int" },
        { name: "product_name", type: "string" },
        { name: "product_id", type: "int" },
         { name: "user_id", type: "int" },
        { name: "user_email", type: "string" },
         { name: "user_name", type: "string" },
        { name: "is_show_name", type: "int" },
        { name: "comment_info", type: "string" },
        { name: "status", type: "int" },
        { name: "create_time", type: "string" },
        { name: "product_desc", type: "int" },
        { name: "seller_server", type: "int" },
        { name: "web_server", type: "int" },
        { name: "logistics_deliver", type: "int" },
        { name: "sender_attitude", type: "int" },
        { name: "brand_name", type: "string" },
        { name: "comment_advice", type: "string" },
        { name: "comment_answer", type: "string" },
    { name: "answer_is_show", type: "int" },
    { name: 's_reply_user', type: "string" },
     { name: 's_reply_time', type: "string" },
    ]
});

var ParameterStore = Ext.create('Ext.data.Store', {
    fields: ['parameterName', 'parameterCode'],
    data: [
        { "parameterName": '食品館', "parameterCode": "10" },
        { "parameterName": '用品館', "parameterCode": "20" }
    ]
});
var ProductCommentStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.ProductCommentModel',
    proxy: {
        type: 'ajax',
        url: '/ProductComment/GetProductCommentList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

secret_info = "user_id;user_name;user_email";
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
var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '好評', "value": "0" },
        { "txt": '中評', "value": "1" },
        { "txt": '差評', "value": "2" }
    ]
});
var ReplayStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '全部', "value": "0" },
        { "txt": '未回覆', "value": "1" },
        { "txt": '已回覆', "value": "2" }
    ]
});
var CommentStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": '非常不滿意', "value": "1" },
        { "txt": '不滿意', "value": "2" },
        { "txt": '一般', "value": "3" },
        { "txt": '滿意', "value": "4" },
        { "txt": '非常滿意', "value": "5" }
    ]
});

//加載前先獲取ddl的值
ProductCommentStore.on('beforeload', function () {
    Ext.apply(ProductCommentStore.proxy.extraParams, {
        ddlSel: Ext.getCmp('ddlSel').getValue(),
        brand_name: Ext.getCmp('brand_name').getValue(),
        productName: Ext.getCmp('productName').getValue(),
        timestart: Ext.getCmp('timestart').getValue(),
        timeend: Ext.getCmp('timeend').getValue(),
        shopClass: Ext.getCmp('shopClass').getValue(),
        productId: Ext.getCmp('productId').getValue(),
        userName: Ext.getCmp('userName').getValue(),
        userEmail: Ext.getCmp('userEmail').getValue(),
        isReplay: Ext.getCmp('isReplay').getValue(),
        commentsel: Ext.getCmp('commentsel').getValue(),
    });
});

function Query(x) {
    Ext.getCmp("pcGift").show();
    ProductCommentStore.removeAll();
    Ext.getCmp("pcGift").store.loadPage(1, {
        params: {
            ddlSel: Ext.getCmp('ddlSel').getValue(),
            brand_name: Ext.getCmp('brand_name').getValue(),
            productName: Ext.getCmp('productName').getValue(),
            timestart: Ext.getCmp('timestart').getValue(),
            timeend: Ext.getCmp('timeend').getValue(),
            shopClass: Ext.getCmp('shopClass').getValue(),
            productId: Ext.getCmp('productId').getValue(),
            userName: Ext.getCmp('userName').getValue(),
            userEmail: Ext.getCmp('userEmail').getValue(),
            isReplay: Ext.getCmp('isReplay').getValue(),
            commentsel: Ext.getCmp('commentsel').getValue()
        }
    });

}
//頁面載入
Ext.onReady(function () {
    var searchForm = Ext.create('Ext.form.Panel', {
        id: 'searchForm',
        layout: 'anchor',
        title: '查詢條件',
        height: 150,
        border: 0,
        bodyPadding: 10,
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
                         margin: "0 5 0 0",
                         name: 'productName',
                         fieldLabel: '商品名稱',
                         labelWidth: 60
                     },
                     {
                         xtype: 'textfield',
                         allowBlank: true,
                         id: 'productId',
                         margin: "0 5 0 0",
                         name: 'productId',
                         fieldLabel: '商品編號',
                         labelWidth: 60
                     },
             {
                 xtype: 'combobox',
                 editable: false,
                 fieldLabel: '好評度',
                 labelWidth: 60,
                 id: 'ddlSel',
                 store: DDLStore,
                 margin: "0 5 0 0",
                 displayField: 'txt',
                 valueField: 'value',
                 emptyValue: '請選擇',
                 labelWidth: 60,
                 listeners: {
                     "select": function (combo, record) {

                     }
                 }
             },
                {
                    xtype: 'textfield',
                    allowBlank: true,
                    id: 'brand_name',
                    margin: "0 5 0 0",
                    name: 'brand_name',
                    fieldLabel: '品牌名稱',
                    labelWidth: 60
                },
             {
                 xtype: 'textfield',
                 allowBlank: true,
                 id: 'userName',
                 margin: "0 5 0 0",
                 name: 'userName',
                 fieldLabel: '用戶名稱',
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
                         xtype: 'textfield',
                         allowBlank: true,
                         id: 'userEmail',
                         margin: "0 5 0 0",
                         name: 'userEmail',
                         fieldLabel: '用戶郵箱',
                         labelWidth: 60
                     },
                    {
                        xtype: 'combobox',
                        editable: false,
                        fieldLabel: '館別',
                        labelWidth: 60,
                        id: 'shopClass',
                        margin: "0 5 0 0",
                        store: ParameterStore,
                        displayField: 'parameterName',
                        valueField: 'parameterCode',
                        emptyValue: '請選擇',
                        labelWidth: 60,
                        listeners: {
                            "select": function (combo, record) {

                            }
                        }
                    },
                 {
                     xtype: 'datetimefield',
                     allowBlank: true,
                     id: 'timestart',
                     margin: "0 5 0 0",
                     name: 'serchcontent',
                     fieldLabel: '評價時間',
                     labelWidth: 60,
                     dateRange: { begin: 'timestart', end: 'timeend' },
                     vtype: 'dateRange'
                 },
             {
                 xtype: 'datetimefield',
                 allowBlank: true,
                 id: 'timeend',
                 margin: "0 5 0 0",
                 name: 'serchcontent',
                 fieldLabel: '到',
                 labelWidth: 60,
                 dateRange: { begin: 'timestart', end: 'timeend' },
                 vtype: 'dateRange'
             },
             {
                 xtype: 'combobox',
                 fieldLabel: '是否回覆',
                 labelWidth: 60,
                 id: 'isReplay',
                 name: 'isReplay',
                 store: ReplayStore,
                 valueField: 'value',
                 displayField: 'txt',
                 value: '1',
                 editable:false
             },
            
            //  {
            //      xtype: 'button',
            //      text: SEARCH,
            //      margin: "0 5 0 0",
            //      iconCls: 'icon-search',
            //      width: 100,
            //      id: 'btnQuery',
            //      handler: Query
            //  },
            //{
            //    xtype: 'button',
            //    text: RESET,
            //    width: 100,
            //    id: 'btn_reset',
            //    listeners: {
            //        click: function () {
            //            Ext.getCmp("brand_name").setValue("");
            //            Ext.getCmp("productName").setValue("");
            //            Ext.getCmp("ddlSel").setValue(null);
            //            Ext.getCmp("timestart").setValue("");
            //            Ext.getCmp("timeend").setValue("");
            //            Ext.getCmp("shopClass").setValue(null);
            //            Ext.getCmp("productId").setValue("");
            //            Ext.getCmp("userName").setValue("");
            //            Ext.getCmp("userEmail").setValue("");
            //            Ext.getCmp("pcGift").hide();

            //        }
            //    }
            //}
                ]
            },
            {
            xtype: 'fieldcontainer',
            combineErrors: true,
            layout: 'hbox',
            items: [
                 {
                     xtype: 'combobox',
                     fieldLabel: '滿意度',
                     labelWidth: 60,
                     editable: false,
                     id: 'commentsel',
                     name: 'commentsel',
                     store: CommentStore,
                     valueField: 'value',
                     displayField: 'txt',
                     //value: '0',
                 },
                ]
              }
        ],
        buttonAlign: 'center',
        buttons: [
            {
                text: '查詢',
                handler: Query,
            },
             {
                 text: '重置',
                 handler: function () {
                     this.up('form').getForm().reset();
                     Ext.getCmp("pcGift").hide();
                 }
             }
        ],
        listeners: {
        }
    });
    //var cellEditingEx = Ext.create('Ext.grid.plugin.CellEditing', {
    //    clicksToEdit: 1
    //});
    var pcGift = Ext.create('Ext.grid.Panel', {
        id: 'pcGift',
        store: ProductCommentStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        hidden: false,
        frame: true,
        flex: 8.8,
        //  plugins: [cellEditingEx],
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
            { header: '編號', dataIndex: 'comment_id', width: 70, align: 'center', align: 'center' },
             { header: '商品編號', dataIndex: 'product_id', width: 70, align: 'center', align: 'center' },
            {
                header: '商品名稱', dataIndex: 'product_name', width: 200, align: 'center'
            },
            {
                header: '品牌名稱', dataIndex: 'brand_name', width: 200, align: 'center'
            },
            {
                header: '用戶名稱', dataIndex: 'user_name', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                    return "<span onclick='SecretLogin(" + record.data.comment_id + "," + record.data.user_id + ",\"" + info_type + "\")'  >" + value + "</span>";

                }
            },
             {
                 header: '用戶郵箱', dataIndex: 'user_email', width: 200, align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {//secretcopy
                     return "<span onclick='SecretLogin(" + record.data.comment_id + "," + record.data.user_id + ",\"" + info_type + "\")'  >" + value + "</span>";

                 }
             },
            {
                header: "留言狀態", dataIndex: 'is_show_name', width: 70, align: 'center',
                renderer: function (value) {
                    if (value == 0) {
                        return Ext.String.format('匿名');
                    }
                    else if (value == 1) {
                        return Ext.String.format('公開');
                    }
                }
            },
            { header: "留言內容", dataIndex: 'comment_info', width: 200, align: 'center', align: 'center' },
            { header: "留言回覆", dataIndex: 'comment_answer', width: 200, align: 'center', align: 'center' },
            {
                header: "回覆是否顯示", dataIndex: 'answer_is_show', width: 100, align: 'center',
                renderer: function (value) {
                    if (value == 1) {
                        return "顯示";
                    }
                    else if (value == 0) {
                        return "<span style='color:red'>隱藏</>";
                    }
                },
                //editor: {
                //    xtype: 'checkboxfield',
                //    width: 40,
                //    labelWidth: 30
                //}
            },
            {
                header: "商品描述相符度", dataIndex: 'product_desc', width: 100, align: 'center',
                renderer: function (value) {
                    if (value == 1) {
                        return Ext.String.format('非常不滿意');
                    }
                    else if (value == 2) {
                        return Ext.String.format('不滿意');
                    }
                    else if (value == 3) {
                        return Ext.String.format('一般');
                    } else if (value == 4) {
                        return Ext.String.format('滿意');
                    } else if (value == 5) {
                        return Ext.String.format('非常滿意');
                    }
                }
            },
            {
                header: '客戶服務滿意度', dataIndex: 'seller_server', width: 100, align: 'center',
                renderer: function (value) {
                    if (value == 1) {
                        return Ext.String.format('非常不滿意');
                    }
                    else if (value == 2) {
                        return Ext.String.format('不滿意');
                    }
                    else if (value == 3) {
                        return Ext.String.format('一般');
                    } else if (value == 4) {
                        return Ext.String.format('滿意');
                    } else if (value == 5) {
                        return Ext.String.format('非常滿意');
                    }
                }
            },
            {
                header: '網站整體服務滿意度', dataIndex: 'web_server', width: 115, align: 'center',
                renderer: function (value) {
                    if (value == 1) {
                        return Ext.String.format('非常不滿意');
                    }
                    else if (value == 2) {
                        return Ext.String.format('不滿意');
                    }
                    else if (value == 3) {
                        return Ext.String.format('一般');
                    } else if (value == 4) {
                        return Ext.String.format('滿意');
                    } else if (value == 5) {
                        return Ext.String.format('非常滿意');
                    }
                }
            },

            {
                header: '配送速度滿意度', dataIndex: 'logistics_deliver', width: 100, align: 'center',
                renderer: function (value) {
                    if (value == 1) {
                        return Ext.String.format('非常不滿意');
                    }
                    else if (value == 2) {
                        return Ext.String.format('不滿意');
                    }
                    else if (value == 3) {
                        return Ext.String.format('一般');
                    } else if (value == 4) {
                        return Ext.String.format('滿意');
                    } else if (value == 5) {
                        return Ext.String.format('非常滿意');
                    }
                }
            },
     //         { name: 's_reply_user', type: "string" },
     //{ name: 's_reply_time', type: "string" },
     { header: '回覆人', dataIndex: 's_reply_user', width: 100, align: 'center' },
     { header: '回覆時間', dataIndex: 's_reply_time', width: 150, align: 'center' },
             {
                 header: '狀態',
                 dataIndex: 'status',
                 align: 'center',
                 id: 'commentControlActive',
                 hidden: true,
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (value == 1) {
                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.comment_id + ")'><img hidValue='0' id='img" + record.data.comment_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                     } else {
                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.comment_id + ")'><img hidValue='1' id='img" + record.data.comment_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                     }
                 }
             }
        ],
        plugins: [
  Ext.create('Ext.grid.plugin.CellEditing', {
      clicksToEdit: 1
  })],

        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: true, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: '回覆', id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            '->',
              {
                  xtype: 'button',
                  text: '匯出CSV',
                  iconCls: 'icon-excel',
                  id: 'btnExcel',
                  handler: function (scroller) {

                      var ddlSel = Ext.getCmp('ddlSel').getValue();
                      var brand_name = Ext.getCmp('brand_name').getValue();
                      var productName = Ext.getCmp('productName').getValue();
                      var timestart = null;
                      if (Ext.getCmp('timestart').getValue() != null) {
                          timestart = new Date(Ext.getCmp('timestart').getValue()).toLocaleDateString();
                      }
                      var timeend = null;
                      if (Ext.getCmp('timeend').getValue() != null) {
                          timeend = new Date(Ext.getCmp('timeend').getValue()).toLocaleDateString();
                      }
                     // var timeend = new Date(Ext.getCmp('timeend').getValue()).toLocaleDateString();
                      var shopClass = Ext.getCmp('shopClass').getValue();
                      var productId = Ext.getCmp('productId').getValue();
                      var userName = Ext.getCmp('userName').getValue();
                      var userEmail = Ext.getCmp('userEmail').getValue();
                      var isReplay = Ext.getCmp('isReplay').getValue();
                      var commentsel = Ext.getCmp('commentsel').getValue();
                      var paras = "?ddlSel=" + ddlSel + "&commentsel=" + commentsel + "&brand_name=" + brand_name + "&productName=" + productName + "&timestart=" + timestart + "&timeend=" + timeend + "&shopClass=" + shopClass + "&productId=" + productId + "&userName=" + userName + "&userEmail=" + userEmail + "&isReplay=" + isReplay;
                      window.open('/ProductComment/IndexExport' + paras);
                  }
                  // ,handler: Query 
              },
            //'->',
            // { xtype: 'textfield', allowBlank: true, id: 'productName', name: 'productName', fieldLabel: '產品名稱', labelWidth: 60 },
            // {
            //     xtype: 'combobox', editable: false, fieldLabel: '好評度', labelWidth: 60, id: 'ddlSel', store: DDLStore, displayField: 'txt', valueField: 'value', emptyValue: '請選擇', labelWidth: 50, listeners: {
            //         "select": function (combo, record) {
            //             //ProductCommentStore.removeAll();
            //             //Ext.getCmp("pcGift").store.loadPage(1, {
            //             //    params: {
            //             //        ddlSel: Ext.getCmp('ddlSel').getValue(),
            //             //        brand_name: Ext.getCmp('brand_name').getValue(),
            //             //        productName: Ext.getCmp('productName').getValue(),
            //             //        timestart: Ext.getCmp('timestart').getValue(),
            //             //        timeend: Ext.getCmp('timeend').getValue()
            //             //    }
            //             //});
            //         }
            //     }
            // },
            // { xtype: 'textfield', allowBlank: true, id: 'brand_name', name: 'brand_name', fieldLabel: '品牌名稱', labelWidth: 60 },
            //  { xtype: 'datetimefield', allowBlank: true, id: 'timestart', name: 'serchcontent', fieldLabel: '評價時間', labelWidth: 60, dateRange: { begin: 'timestart', end: 'timeend' }, vtype: 'dateRange' },
            // { xtype: 'datetimefield', allowBlank: true, id: 'timeend', name: 'serchcontent', fieldLabel: '到', labelWidth: 15, dateRange: { begin: 'timestart', end: 'timeend' }, vtype: 'dateRange' },
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
            //            Ext.getCmp("brand_name").setValue("");
            //            Ext.getCmp("productName").setValue("");
            //            Ext.getCmp("ddlSel").setValue(null);
            //            Ext.getCmp("timestart").setValue("");
            //            Ext.getCmp("timeend").setValue("");
            //        }
                         //    }
            //}
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ProductCommentStore,
            id: 'pagingToolbar',
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
            },
            //edit:function(editor,e){
            //    var comment_id = e.record.data.comment_id;
            //    var value = e.value;
            //    //alert(row_id+" + "+value);
            //    if (e.value != e.originalValue) {
            //        Ext.Ajax.request({
            //            url:'',
            //            params: {
            //                comment_id: comment_id,
            //                value: value,
            //            },
            //            success: function (response) {
            //                var res = Ext.decode(response.responseText);
            //                if (res.success) {
            //                    Ext.Msg.alert("提示信息", "保存成功!");
            //                }
            //            }
            //        });
            //    }
            //}
        },
        selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [searchForm, pcGift],
        renderTo: Ext.getBody(),
       // autoScroll: true,
        listeners: {
            resize: function () {
                pcGift.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //ProductCommentStore.load({ params: { start: 0, limit: 25 } });
});



function SecretLogin(rid, info_id, info_type) {//secretcopy
    var secret_type = "6";//
    var url = "/ProductComment/Index";
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
    editFunction(null, ProductCommentStore);
}
//修改
onEditClick = function () {
    var row = Ext.getCmp("pcGift").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], ProductCommentStore);
    }
}


//更改活動狀態(設置活動可用與不可用)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/ProductComment/UpdateActive",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            ProductCommentStore.remove();
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                ProductCommentStore.load();
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                ProductCommentStore.load();
            }
        },
        error: function (msg) {
            Ext.Msg.alert('111', '222');
        }
    });
}
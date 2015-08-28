var pageSize = 25;

/******************促銷試用活動管理主頁面****************************/
//促銷試用活動管理Model
Ext.define('gigade.PromoAmountTrialModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "id", type: "int" },
        { name: "name", type: "string" },
        { name: "site", type: "string" },
         { name: "group_id", type: "int" },
        { name: "group_name", type: "string" },
        { name: "condition_id", type: "int" },
        { name: "event_img_small", type: "string" },
        { name: "brand_id", type: "int" },
        { name: "brand_name", type: "string" },
        { name: "category_id", type: "int" },
        { name: "category_name", type: "string" }, 
        { name: "product_id", type: "int" },
        { name: "product_name", type: "string" },
         { name: "sale_productid", type: "int" },
           { name: "sale_product_name", type: "string" },
        { name: "market_price", type: "int" },//市價
        { name: "product_img", type: "string" },//商品圖
        { name: "show_number", type: "int" },//試用數量
        { name: "apply_limit", type: "int" },//申請人數上限
        { name: "apply_sum", type: "int" },//報名領取人數
        { name: "device", type: "int" },//設備
        { name: "device_name", type: "string" },//設備
        { name: "event_type", type: "string" },//活動類型[dh]
        { name: "eventtype", type: "string" },//活動類型[zw]
        { name: "event_id", type: "string" },//活動編號
        { name: "event_desc", type: "string" },//活動描述
        { name: "paper_id", type: "int" },
        { name: "paper_name", type: "string" },
        { name: "freight_type", type: "int" },//運送方式
        { name: "freight", type: "string" },//運送方式
        { name: "url", type: "string" },//鏈接
        { name: "event_img", type: "string" },//活動圖
        { name: "active", type: "string" },
        { name: "start_date", type: "datetime" },
        { name: "end_date", type: "datetime" },
        { name: "count_by", type: "string" },
        { name: "num_limit", type: "string" },
        { name: "gift_mundane", type: "string" },
        { name: "repeat", type: "string" },
        { name: "recordCount", type: "int" },
        { name: "shareCount", type: "int" },
        { name: 'muser', type: 'int' },
        { name: 'user_username', type: 'string' }
    ]
});

var PromoAmountTrialStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.PromoAmountTrialModel',
    proxy: {
        type: 'ajax',
        url: '/PromotionsAmountTrial/GetPromotionsAmountTrialList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

Ext.define('gigade.countModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "trial_id", type: "int" },
        { name: "shareCount", type: "int" },
        { name: "recordCount", type: "int" }
    ]
});
//前面選擇框 選擇之後顯示編輯刪除
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdTrial").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdTrial").down('#remove').setDisabled(selections.length == 0);
        }
    }
});

//定義ddl的數據
var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": TEXPIRED, "value": "1" },
        { "txt": NOTPASTDUE, "value": "0" }
    ]
});

//加載前先獲取ddl的值
PromoAmountTrialStore.on('beforeload', function () {
    Ext.apply(PromoAmountTrialStore.proxy.extraParams, {
        ddlSel: Ext.getCmp('ddlSel').getValue(),
        serchcontent: Ext.getCmp('serchcontent').getValue()
    });
});

function Query() {
    PromoAmountTrialStore.removeAll();
    Ext.getCmp("gdTrial").store.loadPage(1, {
        params: {
            ddlSel: Ext.getCmp('ddlSel').getValue(),

            serchcontent: Ext.getCmp('serchcontent').getValue()
        }
    });
}


//頁面載入
var Count;
Ext.onReady(function () {
    var gdTrial = Ext.create('Ext.grid.Panel', {
        id: 'gdTrial',
        store: PromoAmountTrialStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
            { header: HDID, dataIndex: 'event_id', width: 70, align: 'center', align: 'center' },
            {
                header: ACTIVENAME, dataIndex: 'name', width: 100, align: 'center'
            },

               {
                   header: EVENTIMGSMALL, dataIndex: 'event_img',
                   width: 70,
                   align: 'center',
                   xtype: 'templatecolumn',
                   tpl: '<img width=50 name="tplImg"  height=50 src="{event_img_small}" />'
               },

      {
          header: BLANDNAME, dataIndex: 'brand_name', width: 100, align: 'center', hidden: true
      },
            {
                header: PRODNAME, dataIndex: 'product_name', width: 150, align: 'center'
            },
                   {//鏈接至商品介紹頁，不必填
                       header: PRODIMG, dataIndex: 'product_img',
                       width: 70,
                       align: 'center',
                       xtype: 'templatecolumn',
                       tpl: '<a target="_blank" href="{url}" ><img width=50 name="tplImg"  height=50 src="{product_img}" /></a>'
                   },

            { header: "問卷名稱", dataIndex: 'paper_name', width: 100, align: 'center' },
               {
                   header: "URL", dataIndex: 'url', width: 150, align: 'center',
                   renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                       return '<a target="_blank" href="' + value + '" >' + value + '</a>';
                   }
               },
            { header: SHOWNUMBER, dataIndex: 'show_number', width: 80, align: 'center' },
            { header: APPLYLIMIT, dataIndex: 'apply_limit', width: 80, align: 'center' },
              { header: APPLYSUM, dataIndex: 'apply_sum', width: 80, align: 'center' },
            {
                header: GROUPNAME, dataIndex: 'group_name', width: 100, align: 'center', hidden: true,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (record.data.condition_id != 0) {
                        return VIPCONDITION;
                    }
                    else {
                        if (value == "") {
                            return BUFEN;
                        } else {
                            return value;
                        }
                    }
                }
            },

            { header: YSCLASS, dataIndex: 'freight', width: 80, align: 'center', renderer: BufenShow, hidden: true },
            { header: DEVICE, dataIndex: 'device_name', width: 80, align: 'center', renderer: BufenShow, hidden: true },
           { header: ACTIVEDESC, dataIndex: 'event_desc', width: 100, align: 'center' },
           { header: START, dataIndex: 'start_date', width: 120, align: 'center' },
            { header: END, dataIndex: 'end_date', width: 120, align: 'center' },
               {
                   header: VIRTURE,
                   dataIndex: 'id',
                   align: 'center',
                   width: 180,
                   renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                       if (record.data.event_type == "T2") {
                           return '<a href=javascript:TranToDetial2("/PromotionsAmountTrial/TrialRecord","' + value + '","' + record.data.event_id + '")>' + TRIALRECORD + "(" + record.data.recordCount + ")" + '</a>  &nbsp; &nbsp;<a href=javascript:TranToDetial("/PromotionsAmountTrial/ShareRecord","' + value + '","' + record.data.event_id + '")>' + SHARERECORD + "(" + record.data.shareCount + ")" + '</a> ';
                       }
                       else if (record.data.event_type == "T1") {
                           return '<a href=javascript:TranToDetial2("/PromotionsAmountTrial/TrialRecord","' + value + '","' + record.data.event_id + '")>' + FORETASTERECODE + "(" + record.data.recordCount + ")" + '</a> &nbsp; &nbsp;<a href=javascript:TranToDetial("/PromotionsAmountTrial/ShareRecord","' + value + '","' + record.data.event_id + '")>' + SHARERECORD + "(" + record.data.shareCount + ")" + '</a> ';
                       }
                   }
               },
               { header: 'muser', dataIndex: 'muser', hidden: true },
               { header: QuanXianMuser, dataIndex: 'user_username', width: 80, align: 'center' },
            {
                header: ISACTIVE,
                dataIndex: 'active',
                align: 'center',
                width: 70,
                id: 'controlactive',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {

                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.id + "," + record.data.muser + ")'><img hidValue='0' id='img" + record.data.id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.id + "," + record.data.muser + ")'><img hidValue='1' id='img" + record.data.id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            { xtype: 'button', text: REMOVE, id: 'remove', hidden: true, iconCls: 'icon-user-remove', disabled: true, handler: onRemoveClick },
            '->',
             {
                 xtype: 'textfield', allowBlank: true, id: 'serchcontent', name: 'serchcontent', fieldLabel: PROMOSITIONNAMECODE, labelWidth: 100,
                 listeners: {
                     specialkey: function (field, e) {
                         if (e.getKey() == e.ENTER) {
                             Query();
                         }
                     }
                 }
             },
            {
                xtype: 'combobox', editable: false, fieldLabel: OPTION, labelWidth: 60, id: 'ddlSel', store: DDLStore, displayField: 'txt', valueField: 'value', value: '0', labelWidth: 50

            },
            {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: PromoAmountTrialStore,
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
        layout: 'fit',
        items: [gdTrial],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdTrial.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // PromoAmountTrialStore.load({ params: { start: 0, limit: 25 } });
});




function BufenShow(val) {
    switch (val) {
        case "":
            return BUFEN;
            break;
        default:
            return val;
            break;
    }
}


//添加
onAddClick = function () {
    editFunction(null, PromoAmountTrialStore);
}
//修改
onEditClick = function () {
    var row = Ext.getCmp("gdTrial").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], PromoAmountTrialStore);
    }
}
//刪除
onRemoveClick = function () {
    var row = Ext.getCmp("gdTrial").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.id + '|';
                }
                Ext.Ajax.request({
                    url: '/PromotionsAmountTrial/DeletePromoAmountTrial',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);

                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        PromoAmountTrialStore.load(1);
                        if (result.success) {
                            PromoAmountTrialStore.load(1);
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        });
    }
}

//更改活動狀態(設置活動可用與不可用)
function UpdateActive(id, muser) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/PromotionsAmountTrial/UpdateActive",
        data: {
            "id": id,
            "muser": muser,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (msg.success == "stop") {
                Ext.Msg.alert("提示信息", QuanXianInfo);
            }
            else if (msg.success == "true") {
                PromoAmountTrialStore.removeAll();
                PromoAmountTrialStore.load();
                if (activeValue == 1) {
                    $("#img" + id).attr("hidValue", 0);
                    $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                } else {
                    $("#img" + id).attr("hidValue", 1);
                    $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                }
            }
            else {
                Ext.Msg.alert("提示信息", "更改失敗");
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, EDITERROR);
        }
    });
}


function TranToDetial(url, id, event_id) {
    var RECORD = "";
    if (event_id.indexOf("T2") >= 0) {
        RECORD = TRIALRECORD;
    }
    else if (event_id.indexOf("T1") >= 0) {
        RECORD = FORETASTERECODE;
    }
    var urlTran = url + '?id=' + id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#detial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'detial',
        title: url == '/PromotionsAmountTrial/ShareRecord' ? SHARERECORD : RECORD,
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}

function TranToDetial2(url, id, event_id) {
    var RECORD = "";
    if (event_id.indexOf("T2") >= 0) {
        RECORD = TRIALRECORD;
    }
    else if (event_id.indexOf("T1") >= 0) {
        RECORD = FORETASTERECODE;
    }
    var urlTran = url + '?id=' + id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#detial2');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'detial2',
        title: url == '/PromotionsAmountTrial/ShareRecord' ? SHARERECORD : RECORD,
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}
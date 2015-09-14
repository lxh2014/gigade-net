var pageSize = 25;

Ext.define('gigade.Ipod', {
    extend: 'Ext.data.Model',
    fields: [
     { name: "row_id", type: "int" },
     { name: "po_id", type: "string" },
     { name: "pod_id", type: "int" },
     { name: "ParameterCode", type: "string" },
     { name: "bkord_allow", type: "string" },
     { name: "cde_dt_incr", type: "int" },
     { name: "cde_dt_var", type: "int" },
     { name: "cde_dt_shp", type: "int" },
     { name: "pwy_dte_ctl", type: "string" },
     { name: "qty_ord", type: "int" },
     { name: "qty_damaged", type: "int" },
     { name: "qty_claimed", type: "int" },
     { name: "promo_invs_flg", type: "string" },
     { name: "req_cost", type: "string" },
     { name: "off_invoice", type: "string" },
     { name: "new_cost", type: "string" },
     { name: "freight_price", type: "int" },
     { name: "prod_id", type: "string" },
     { name: "create_user", type: "string" },
     { name: "create_dtim", type: "string" },
     { name: "user_username", type: "string" },
      { name: "parameterName", type: "string" }
    ]
});

//
//採購單單身store
var IpodStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Ipod',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetIpodList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
IpodStore.on('beforeload', function () {
    Ext.apply(IpodStore.proxy.extraParams,
        {
            ipod: Ext.getCmp('searchcontent').getValue()
        });
});
Ext.define("gigade.parameter", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ParameterCode", type: "string" },
        { name: "parameterName", type: "string" }
    ]
});
var PromoInvsFlgStore = Ext.create('Ext.data.Store', {
    model: 'gigade.parameter',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/WareHouse/GetPromoInvsFlgList?Type=promo_invs_flg",
        //     actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
var PlstIdStore = Ext.create('Ext.data.Store', {
    model: 'gigade.parameter',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/WareHouse/GetPlstIdList?Type=plst_id",
        //     actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("IpodView").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("IpodView").down('#delete').setDisabled(selections.length == 0);
        }
    }
});
function Query(x) {
    IpodStore.removeAll();
    Ext.getCmp("IpodView").store.loadPage(1, {
        params: {
            ipod: Ext.getCmp('searchcontent').getValue()
        }

    });
}

Ext.onReady(function () {


    var IpodView = Ext.create('Ext.grid.Panel', {
        id: 'IpodView',
        store: IpodStore,
        flex: 8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
        { header: "商品細項編號", dataIndex: 'prod_id', width: 100, align: 'center' },
        { header: "收貨狀態", dataIndex: 'parameterName', width: 100, align: 'center' },
         { header: "收貨狀態", dataIndex: 'ParameterCode', width: 100, align: 'center', hidden: true },
           

      
        {
            header: "下單採購量", dataIndex: 'qty_ord', width: 100, align: 'center'
        },
        { header: "是否允許多次收貨", dataIndex: 'bkord_allow', width: 120, align: 'center' },
        { header: "不允收的量", dataIndex: 'qty_damaged', flex: 1, align: 'center' },
        { header: "實際收貨量", dataIndex: 'qty_claimed', flex: 1, align: 'center' },
        { header: "品項庫存用途", dataIndex: 'promo_invs_flg', flex: 1, align: 'center' },
        { header: "訂貨價格", dataIndex: 'new_cost', flex: 1, align: 'center' },
        { header: "運費", dataIndex: 'freight_price', flex: 1, align: 'center' },
        { header: "是否有效期控管", dataIndex: 'pwy_dte_ctl', flex: 1, align: 'center' },
        { header: "有效期天數", dataIndex: 'cde_dt_incr', flex: 1, align: 'center' },
        { header: "允收天數", dataIndex: 'cde_dt_var', flex: 1, align: 'center' },
        { header: "允出天數", dataIndex: 'cde_dt_shp', flex: 1, align: 'center' },
        { header: "創建人", dataIndex: 'user_username', flex: 1, align: 'center' },
        { header: "創建時間", dataIndex: 'create_dtim', flex: 1, align: 'center' }
        ],
        tbar: [
          //{ xtype: 'button', text: "新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
          { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
          { xtype: 'button', text: "刪除", id: 'delete', iconCls: 'icon-user-remove', disabled: true, handler: onDeleteClick }
          ,
         // "->",
         {
             id: 'searchcontent',
             xtype: 'textfield',
             fieldLabel: "採購單單號",
             maxLength:18,
             labelWidth: 70,
             //regex: /^\d+$/,
             //regexText: '请输入正確的編號,訂單編號,行動電話進行查詢',
             name: 'searchcontent',
             allowBlank: true,
             hidden: true,
             value: document.getElementById("Ipod_poid").value
         }
         ,
         //   {
         //       xtype: 'button',
         //       text: SEARCH,

         //       margin: '0 0 0 0',
         //       id: 'btnQuery',
         //       iconCls: 'icon-search',
         //       handler: Query
         //   },
         //   {
         //       xtype: 'button',
         //       text: RESET,
         //       margin: '0 0 0 0',
         //       id: 'btn_reset',
         //       iconCls: 'ui-icon ui-icon-reset',
         //       listeners: {
         //           click: function () {
         //               Ext.getCmp("searchcontent").setValue('');

         //           }
         //       }
         //   }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: IpodStore,
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
        }, selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [IpodView],//
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                IpodView.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    IpodStore.load({ params: { start: 0, limit: 25 } });
});

/*******************************新增*********************************************/
onAddClick = function () {

    editFunction(null, IpodStore);
}

/************************編輯*********************************/
onEditClick = function () {
    var row = Ext.getCmp("IpodView").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], IpodStore);
    }
}
/************************刪除**************************/
onDeleteClick = function () {
    var row = Ext.getCmp("IpodView").getSelectionModel().getSelection();
        if (row.length <= 0) {
            Ext.Msg.alert(INFORMATION, NO_SELECTION);
        } else {
            Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
                if (btn == 'yes') {
                    var rowIDs = '';
                    for (var i = 0; i < row.length; i++) {
                        rowIDs += row[i].data.row_id + ',';
                    }

                    Ext.Ajax.request({
                        url: '/WareHouse/DeleteIpod',
                        method: 'post',
                        params: {
                            rowId: rowIDs
                        },
                        success: function (form, action) {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            IpodStore.load();

                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                            IpodStore.load();
                        }
                    });
                }
            });
        }
}


function TransToUser(UserMobile) {
    var url = '/Member/UsersListIndex?UserMobile=' + UserMobile.split('*')[0];
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#user_detial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'user_detial',
        title: '會員內容',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    } else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}



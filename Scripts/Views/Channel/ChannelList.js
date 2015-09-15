/*
* 文件名稱 :ChannelList.js
* 文件功能描述 :外站諮詢表
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/19
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

var pageSize = 25;

//群組管理Model
Ext.define('gigade.Channel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "channel_id", type: "string" },
        { name: "channel_name_full", type: "string" },
        { name: "channel_name_simple", type: "string" },
        { name: "channel_status_name", type: "string" },
        { name: "erp_id", type: "string" },//add by xiangwang0413w 2014/06/26 增加 ERP客戶代號
        { name: 'channel_type', type: 'string' },
        { name: 'receipt_to', type: 'string'}]
});

var ChannelListStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize:pageSize,
    model: 'gigade.Channel',
    proxy: {
        type: 'ajax',
        url: '/Channel/Query',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items',
            totalProperty: 'total'
        }
    }
});

ChannelListStore.on('beforeload', function () {
    Ext.apply(ChannelListStore.proxy.extraParams,
        {
            ddlSel: Ext.getCmp('ddlSel').getValue(),
            txtSel: Ext.getCmp('txtSel').getValue(),
            cobStatus: Ext.getCmp('ddlStatus').getValue()
        });
});

var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": FULLNAME, "value": "full" },
        { "txt": SIMPLENAME, "value": "simple" }
    ]
});

var DDLStatusStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": STATUS_ALL, "value": "0" },
        { "txt": STATUS_START, "value": "1" },
        { "txt": STATUS_STOP, "value": "2" }
    ]
});

var channelTpl = new Ext.XTemplate(
    '<a href="/channel?id={channel_id}">{channel_name_full}</a>'
);

/*將漢字轉化為圖標 eg.:啟用=》圖標*/
function beforeRender(value) {
    if (value == '啟用') {
        return "<img src='../../../Content/img/icons/accept.gif' />";
    }
    else {
        return "<img src='../../../Content/img/icons/drop-no.gif' />";
    }
}

/**end**/

Ext.onReady(function () {
    var gdChannelList = Ext.create('Ext.grid.Panel', {
        id: 'gdChannelList',
        store: ChannelListStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: ID, xtype: 'rownumberer', width: 100, align: 'center' },
            { header: ROWNUM, dataIndex: 'channel_id', align: 'center', hidden: true },
            { header: FULLNAME, xtype: 'templatecolumn', tpl: channelTpl, align: 'center' },
            { header: SIMPLENAME, dataIndex: 'channel_name_simple', width: 150, align: 'center' },
            { header: CHANNELTYPE, dataIndex: 'channel_type', width: 150, align: 'center',
                renderer: function (value) {
                    if (value == '1') {
                        return CHANNEL_TYPE_COOPERATION;
                    }
                    else if (value == '2') {
                        return CHANNEL_TYPE_GIGADE;
                    }
                    else {
                        return CHNANEL_TYPE_OTHERS;
                    }
                }
            },
            { header: CHANNELRECEIPT, dataIndex: 'receipt_to', width: 150, align: 'center',
                renderer: function (value) {
                    switch (value) {
                        case '1': return RECEIPT_TO_1; break;
                        case '2': return RECEIPT_TO_2; break;
                        case '3': return RECEIPT_TO_3; break;
                        default: return RECEIPT_TO_OTHERS; break;
                    }
                }
            },
            { header: CHANNELERPID, dataIndex: 'erp_id', width: 150, align: 'center' },//add by xiangwang0413w 2014/06/26 增加 ERP客戶代號
            { header: CHANNELSTATUS, dataIndex: 'channel_status_name', width: 150, align: 'center', renderer: beforeRender },
            ],
        tbar: [{
            xtype: 'combobox', editable: false, fieldLabel: DDLSEL, labelWidth: 60, id: 'ddlSel', store: DDLStore, displayField: 'txt', valueField: 'value', value: 'full'
        }, {
            xtype: 'textfield', fieldLabel: SELCONTENT, id: 'txtSel'
        }, {
            xtype: 'combobox', editable: false, fieldLabel: CHANNELSTATUS, labelWidth: 60, id: 'ddlStatus', store: DDLStatusStore, displayField: 'txt', valueField: 'value', value: '0'
        }, {
            text: SELECT,
            iconCls: 'ui-icon ui-icon-search-2',
            id: 'btnQuery',
            handler: Query
        }],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }, bbar: Ext.create('Ext.PagingToolbar', {
            store: ChannelListStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })

    });
    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdChannelList],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdChannelList.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

    ChannelListStore.load();
});

function Query(x) {
    ChannelListStore.removeAll();
    ChannelListStore.loadPage(1);
    Ext.getCmp("gdChannelList").store.load({
        params: {
            ddlSel: Ext.getCmp('ddlSel').getValue(),
            txtSel: Ext.getCmp('txtSel').getValue(),
            cobStatus: Ext.getCmp('ddlStatus').getValue()
        }
    });
    Ext.getCmp('txtSel').reset();
}

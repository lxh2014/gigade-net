Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var CallidForm;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.define('gigade.Serial', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "myid", type: "int" },
        { name: "serial", type: "string" },
        { name: "active", type: "string" }
    ]
});

var FaresStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.Serial',
    proxy: {
        type: 'ajax',
        url: '/PromotionsBonus/PromotionsBonusSerialLists',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

FaresStore.loadPage(1, {
    params: {
        ids: document.getElementById('SerialId').value
    }
});
Ext.onReady(function () {
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: FaresStore,
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
            { header: '編號', dataIndex: 'myid', flex: 1, align: 'center', align: 'center' },
            { header: XVHAO, dataIndex: 'serial', flex: 2, align: 'center' },
            { header: 'ACTIVE', dataIndex: 'active', flex: 1, align: 'center' }
           ],
        tbar: [
            { xtype: 'textfield', editable: false, fieldLabel: '序號數量', labelWidth: 80, id: 'xhsl', name: 'xhsl' },
            { xtype: 'numberfield', editable: true, minValue: 5, maxValue: 32, fieldLabel: '序號長度', labelWidth: 80, id: 'xhcd', name: 'xhcd', value: 8 },
            { text: XHADD, iconCls: 'icon-add', id: 'btnenter', handler: Inters },
            { text: '匯出', iconCls: 'icon-excel', id: 'btnExcel', handler: ExportCSV }
           
        ],
          dockedItems: [
            {   //類似于tbar
                xtype: 'toolbar',
                dock: 'top',
                items: [
                   {
                       xtype:'label',
                       text:'檢視序號',
                       margins: '0 0 0 10'
                   }, '->',
                  { xtype: "button", text: "返回序號兌換列表", id: "goback", handler: function () {
                  window.location.href = "/PromotionsBonus/index";
              }
              }
                ]
            }]
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdFgroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdFgroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
});
function Inters(x) {
    //    FaresStore.removeAll();
    //    FaresStore.loadPage(1);
    Ext.Ajax.request({
        url: '/PromotionsBonus/InsertPromotionsBonusSerial',
        params: {
            xhsl: Ext.getCmp('xhsl').getValue(),
            xhcd: Ext.getCmp('xhcd').getValue(),
            ids: document.getElementById('SerialId').value
        },
        success: function (response) {
            FaresStore.removeAll();
            FaresStore.loadPage(1, {
                params: {
                    ids: document.getElementById('SerialId').value
                }
            })
        }
    });
}
//數據匯出

ExportCSV = function () {
    Ext.MessageBox.show({
        msg: 'Loding....',
        wait: true
    });
    Ext.Ajax.request({
        url: "/PromotionsBonus/PromotionsBonusSerialListsExport",
        timeout: 900000,
        params: {
            ids: document.getElementById('SerialId').value
        },
        success: function (response) {
            if (response.responseText == "true") {
                window.location = '../../ImportUserIOExcel/promotions_bonus_serial.csv';
                Ext.MessageBox.hide();
            } else {
                Ext.MessageBox.hide();
                Ext.Msg.alert(INFORMATION, "匯出失敗或沒有數據,請先搜索查看!");
            }
        }
    });
}
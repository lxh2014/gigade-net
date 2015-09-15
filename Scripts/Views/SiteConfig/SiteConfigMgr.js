
Ext.define('GIGADE.SiteConfig', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Name', type: 'string' },
        { name: 'Remark', type: 'string' },
        { name: 'Value', type: 'string' },
        { name: 'DefaultValue', type: 'string' },
    ]
});

var configStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.SiteConfig',
    proxy: {
        type: 'ajax',
        url: '/SiteConfig/QueryConfig',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    },
    listeners: {
        update: function (store, record) {
            if (record.isModified('Value')) {
                Ext.Ajax.request({
                    url: '/SiteConfig/UpdateConfig',
                    params: {
                        Name: record.get("Name"),
                        Value: Ext.htmlEncode(record.get('Value'))
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (!result.success) {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    }
                });
            }
        }
    }
});

var rowEditing = Ext.create('Ext.grid.plugin.RowEditing', {
    clicksToMoveEditor: 1,
    autoCancel: false,
    errorSummary: false
});

Ext.onReady(function () {

    Ext.grid.RowEditor.prototype.saveBtnText = SAVE;
    Ext.grid.RowEditor.prototype.cancelBtnText = CANCEL;

    var configGrid = Ext.create('Ext.grid.Panel', {
        id: 'configGrid',
        store: configStore,
        width: document.documentElement.clientWidth,
        plugins: [rowEditing],
        columnLines: true,
        frame: true,
        columns: [
            { header: NUMBER, xtype: 'rownumberer', width: 100, align: 'center' },
            { header: CONFIG_KEY, dataIndex: 'Name', width: 140, align: 'center' },
            { header: CONFIG_VALUE, dataIndex: 'Value', width: 180, align: 'center',
                editor: {
                    xtype: 'textfield'
                }
            },
            { header: CONFIG_DEFAULT_VALUE, dataIndex: 'DefaultValue', width: 180, align: 'center' },
            { header: CONFIG_REMARK, dataIndex: 'Remark', width: 200, align: 'left' }
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

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [configGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                configGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

    configStore.load();
});
var tabs;
function groupAuthority(groupId) {
    tabs = new Array();
    viewAuthority(groupId);
    editAuthority(groupId);

    var powerForm = Ext.create('Ext.tab.Panel', {
        id: 'ContentPanel',
        //width: 1185,
        //minTabWidth: 100,
        autoScroll: true,
        tabPosition: 'bottom',
        activeTab: 0,
        items: [tabs]
    });

    /****************************************************************************************/


    Ext.create('Ext.window.Window', {
        title: TOOL_AUTHORITY,
        items: [powerForm],
        width: 500,
        height: document.documentElement.clientHeight * 400 / 783,
        layout: 'fit',
        autoScroll: true,
        frame: false,
        closeAction: 'destroy',
        //resizable: false,
        //draggable: false,
        modal: true//,

    }).show();
};



//查看權限
function viewAuthority(groupId) {
    //權限store
    var AuthorityStore = Ext.create('Ext.data.Store', {
        fields: ['FunctionGroup', 'items'],
        proxy: {
            type: 'ajax',
            url: '/FunctionGroup/GetFgroupAuthority',
            actionMethods: 'post',
            reader: {
                type: 'json'
            }
        }
    });
    //權限框
    var AuthView = Ext.create('Ext.view.View', {
        deferInitialRefresh: false,
        autoScroll: true,
        frame: false,
        plain: true,
        store: AuthorityStore,
        tpl: Ext.create('Ext.XTemplate',
            '<div id="AuthView" class="View">',
            '<tpl for="."><div class="group">',
                '<h2><div>{FunctionGroup}</div></h2>',
                '<dl>',
                '<tpl for="items">',
                    '<div class="name downline">',
                        '<input type="checkbox" id="{RowId}" onclick="pageCancel(this)" name="{RowId}" {[values.checked=="True"?"checked=checked":""]} />&nbsp;<label for="{RowId}">{Name}</label><br/>',
                        '<div class="tool {[values.tools.length>0?"downline":""]}" style="float:left;">',
                            '<tpl for="tools">',
                                '<input type="checkbox" id="{RowId}" onclick="toolClick(this)" name="{RowId}" {[values.checked=="True"?"checked=checked":""]} />&nbsp;<label for="{RowId}">{Name}</label> ',
                                '{[xindex % 5==0?"<br/>":""]}',
                            '</tpl>',
                        '</div>',
                    '</div>',
                '</tpl>',
                '<div style="clear:left"></div></dl>',
            '</div></tpl>',
            '</div>'
        ),
        onContainerClick: function (e) {
            var group = e.getTarget('h2', 3, true);

            if (group) {
                group.up('div').toggleCls('collapsed');
            }
        },
        itemSelector: 'dl',
        overItemCls: 'group-hover'
    });

    //查看權限
    var viewPanel = Ext.create('Ext.panel.Panel', {
        title: WATCH_AUTHORITY,//查看權限
        autoScroll: true,
        items: [AuthView],
        bbar: [{
            xtype: 'checkbox',
            boxLabel: CHECK_ALL,
            listeners: {
                change: function (e, newValue) {
                    $('#AuthView').find('input:checkbox').each(function () {
                        if (newValue) {
                            $(this).attr('checked', 'checked');
                        }
                        else {
                            $(this).removeAttr('checked');
                        }
                    });
                }
            }
        },
        '->',
        {
            xtype: 'button',
            text: SUBMIT,
            handler: function () {
                var chks = $('#AuthView').find(':checked');
                if (chks.length == 0) {
                    Ext.Msg.alert(INFORMATION, NO_DATA);
                    return;
                }
                var mask;
                if (!mask) {
                    mask = new Ext.LoadMask(Ext.getBody());
                }
                mask.show();
                var rowIds = '';
                chks.each(function () {
                    rowIds += $(this).attr('id') + '|';
                });
                Ext.Ajax.request({
                    url: '/FunctionGroup/SaveAuthority',
                    method: 'post',
                    timeout:720000,
                    params: { GroupId: groupId, rowID: rowIds },
                    success: function (form, action) {
                        mask.hide();
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, SET_AUTHORTIY_SUCCESS);
                            //AuthorityStore.load({ params: { RowId: groupId } });
                            Ext.getCmp('editAuthView').getStore().load({ params: { RowId: groupId } });
                        } else {
                            Ext.Msg.alert(INFORMATION, SET_AUTHORTIY_FAILURE);
                        }
                    },
                    failure: function () {
                        mask.hide();
                        Ext.Msg.alert(INFORMATION, SET_AUTHORTIY_FAILURE);
                    }
                })
            }
        }, {
            xtype: 'button',
            text: RESET,
            handler: function () {
                AuthorityStore.removeAll();
                AuthorityStore.load({ params: { RowId: groupId } });
            }
        }]
    });
    tabs.push(viewPanel);
    AuthorityStore.load({ params: { RowId: groupId } });
}

//修改權限
function editAuthority(groupId) {
    //Store
    var editAuthStore = Ext.create('Ext.data.Store', {
        fields: ['FunctionGroup', 'items'],
        proxy: {
            type: 'ajax',
            url: '/FunctionGroup/GetFgroupAuthority?type=2',
            actionMethods: 'post',
            reader: {
                type: 'json'
            }
        }
    });

    //View
    var editAuthView = Ext.create('Ext.view.View', {
        id: 'editAuthView',
        deferInitialRefresh: false,
        autoScroll: true,
        frame: false,
        plain: true,
        store: editAuthStore,
        tpl: Ext.create('Ext.XTemplate',
            '<div id="editAuthView" class="View">',
            '<tpl for="."><div class="group">',
                '<h2><div>{FunctionGroup}</div></h2>',
                '<dl>',
                '<tpl for="items">',
                    '<div class="name downline">',
                        '&nbsp;<label >{Name}</label><br/>',
                        '<div class="tool {[values.tools.length>0?"downline":""]}" style="float:left;">',
                            '<tpl for="tools">',
                                '<input type="checkbox" id="edit_{RowId}" name="{RowId}" onclick="toolClick(this)" {[values.checked=="True"?"checked=checked":""]} />&nbsp;<label for="edit_{RowId}">{Name}</label> ',
                                '{[xindex % 5==0?"<br/>":""]}',
                            '</tpl>',
                        '</div>',
                    '</div>',
                '</tpl>',
                '<div style="clear:left"></div></dl>',
            '</div></tpl>',
            '</div>'
        ),
        onContainerClick: function (e) {
            var group = e.getTarget('h2', 3, true);

            if (group) {
                group.up('div').toggleCls('collapsed');
            }
        },
        itemSelector: 'dl'
    });

    //修改權限Panel
    var editPanel = Ext.create('Ext.panel.Panel', {
        title: EDIT_AUTHORITY,//修改權限
        autoScroll: true,
        items: [editAuthView],
        bbar: [{
            xtype: 'checkbox',
            boxLabel: CHECK_ALL,
            listeners: {
                change: function (e, newValue) {
                    $('#editAuthView').find('input:checkbox').each(function () {
                        if (newValue) {
                            $(this).attr('checked', 'checked');
                        }
                        else {
                            $(this).removeAttr('checked');
                        }
                    });
                }
            }
        },
            '->',
            {
                xtype: 'button',
                text: SUBMIT,
                handler: function () {
                    var chks = $('#editAuthView').find(':checked');
                    if (chks.length == 0) {
                        Ext.Msg.alert(INFORMATION, NO_DATA);
                        return;
                    }
                    var mask;
                    if (!mask) {
                        mask = new Ext.LoadMask(Ext.getBody());
                    }
                    mask.show();
                    var rowIds = '';
                    chks.each(function () {
                        rowIds += $(this).attr('name') + '|';
                    });
                    Ext.Ajax.request({
                        url: '/FunctionGroup/UpdateEidtFunction',
                        method: 'post',
                        params: { GroupId: groupId, rowID: rowIds },
                        success: function (form, action) {
                            mask.hide();
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SET_AUTHORTIY_SUCCESS);
                                //editAuthStore.load({ params: { RowId: groupId } });
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, SET_AUTHORTIY_FAILURE);
                            }
                        },
                        failure: function () {
                            mask.hide();
                            Ext.Msg.alert(INFORMATION, SET_AUTHORTIY_FAILURE);
                        }
                    })
                }
            }, {
                xtype: 'button',
                text: RESET,
                handler: function () {
                    editAuthStore.removeAll();
                    editAuthStore.load({ params: { RowId: groupId } });
                }
            }]
    });
    tabs.push(editPanel);
    editAuthStore.load({ params: { RowID: groupId } });
}


//給予按鈕權限時，頁面權限同時給予
function toolClick(e) {
    var chk = $(e);
    var prev = chk.parent().prevAll('input:checkbox');
    if (chk.attr('checked') && !prev.attr('checked')) {
        prev.attr('checked', 'checked');
    }
}
//頁面權限取消時，頁面按鈕權限同時取消
function pageCancel(e) {
    var chk = $(e);
    chk.nextAll('.tool').children('input:checkbox').each(function () {
        $(this).removeAttr('checked');
    });
}
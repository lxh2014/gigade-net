var boolstatus = true;
editFunction = function (row, store) {

    Ext.define("gigade.ElementType", {
        extend: 'Ext.data.Model',
        fields: [
            { name: "ParameterCode", type: "string" },
            { name: "parameterName", type: "string" }]
    });
    var ElementTypeStore = Ext.create('Ext.data.Store', {
        model: 'gigade.ElementType',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/Element/GetElementType",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }

        }
    });

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/Element/AreaSave',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
        {
            xtype: 'textfield',
            fieldLabel: 'area_id',
            id: 'area_id',
            name: 'id',
            hidden: true
        },
        //start
             //end
        {
            xtype: 'textfield',
            fieldLabel: AREANAME,
            name: 'name',
            id: 'area_name',
            allowBlank: false
        },
           {
               xtype: 'combobox',
               fieldLabel: ELEMENTTYPE,
               id: 'element_type',
               name: 'element_type',
               lastQuery: '',
               allowBlank: false,
               editable: false,
               typeAhead: true,
               forceSelection: false,
               store: ElementTypeStore,
               displayField: 'parameterName',
               valueField: 'ParameterCode',
               emptyText: SELECT
           },
          {
              xtype: 'numberfield',
              fieldLabel: SHOWNUMBER,
              name: 'show_number',
              id: 'show_number',
              allowBlank: false,
              value: 0,
              minValue: 0
          },
        {
            xtype: 'textareafield',
            fieldLabel: ESCINFO,
            name: 'area_desc',
            id: 'area_desc',
            anchor: '100%'
        },
         {
             xtype: 'textareafield',
             fieldLabel: AREAID,
             name: 'area_element_id',
             id: 'area_element_id',
             anchor: '100%',
             regex:/^\w+$/,
             regexText: AREAELEMENTIDTIP

         }
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {

                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            id: Ext.htmlEncode(Ext.getCmp('area_id').getValue()),
                            name: Ext.htmlEncode(Ext.getCmp('area_name').getValue()),
                            area_desc: Ext.htmlEncode(Ext.getCmp('area_desc').getValue()),
                            show_number: Ext.htmlEncode(Ext.getCmp('show_number').getValue()),
                            area_element_id: Ext.htmlEncode(Ext.getCmp('area_element_id').getValue()),
                            element_type: Ext.htmlEncode(Ext.getCmp('element_type').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                          
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                ElementStores.load();
                                editWin.close();
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
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: AREATITLE,
        id: 'editWin',
        iconCls: 'icon-user-edit',
        width: 420,
        layout: 'fit',
        items: [editFrm],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        constrain: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSE,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function () {
                if (row == null) {
                    editFrm.getForm().reset(); //如果是添加的話

                } else {

                    editFrm.getForm().loadRecord(row); //如果是編輯的話
                }
            }
        }
    });
    editWin.show();
}

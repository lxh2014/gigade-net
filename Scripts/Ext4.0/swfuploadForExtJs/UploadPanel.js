


Ext.define("Org.fileupload.FileModel",{
	extend:"Ext.data.Model",
	fields : [ 'id', 'name', 'type', 'size', 'filestatus', 'percent' ]

});


Ext.define("Org.fileupload.Panel", {
	extend : "Ext.panel.Panel",
	alias : "widget.fileuploadPanel",
	layout:"fit",
	initComponent : function() {
		this.width=510;
		this.height=200;
		this.continuous=false;//是否连续上传，true为连续上传队列后其他文件,false只上传当前队列开始的文件
		this.setting={
				upload_url : this.uploadUrl, 
				flash_url : this.flashUrl,
				file_size_limit : this.fileSize || (1024*50) ,//上传文件体积上限，单位MB
				file_post_name : this.filePostName,
				file_types : this.fileTypes||"*.*",  //允许上传的文件类型 
		        file_types_description : "All Files",  //文件类型描述
		        file_upload_limit : "0",  //限定用户一次性最多上传多少个文件，在上传过程中，该数字会累加，如果设置为“0”，则表示没有限制 
		        file_queue_limit : "10",//上传队列数量限制，该项通常不需设置，会根据file_upload_limit自动赋值              
				post_params : this.postParams||{savePath:'upload\\'},
				use_query_string : true,
				debug : true,
				button_cursor : SWFUpload.CURSOR.HAND,
				button_window_mode : SWFUpload.WINDOW_MODE.TRANSPARENT,
				custom_settings : {//自定义参数
					scope_handler : this
				},
				swfupload_loaded_handler : function(){
					console.log("swf组件成功初始化");
				},// 当Flash控件成功加载后触发的事件处理函数
				file_dialog_start_handler : function(){},// 当文件选取对话框弹出前出发的事件处理函数
				file_dialog_complete_handler : function(){},//当文件选取对话框关闭后触发的事件处理
				upload_start_handler : function(){},// 开始上传文件前触发的事件处理函数
				upload_success_handler : function(){},// 文件上传成功后触发的事件处理函数 
				upload_progress_handler :this.uploadProgress,
				upload_complete_handler : this.uploadComplete,
				upload_error_handler : this.onFileError,
				file_queue_error_handler :  this.onFileError,
				file_queued_handler:this.onQueued
		};
		this.items = [ {
			listeners:{
				'itemcontextmenu':function(myself,model,item,index,e,eopt){
					var rightClick = Ext.create('Ext.menu.Menu', {
										autoDestroy:true,
										items : [{
													scope:this.ownerCt,
													text : '开始上传',
													handler:function(){
														console.log("点击开始上传");
														this.swfupload.customSettings.scope_handler.continuous=false;
														this.swfupload.startUpload(model.get("id"));
														console.log(model.get("id"));
														model.set("filestatus",-2);
													},
										icon: "../../../../Scripts/Ext4.0/swfuploadForExtJs/image/icons/upload.gif"
												}, {
													text : '取消上传',
													scope:this.ownerCt,
													handler:function(){
														this.swfupload.cancelUpload(model.get("id"),false);
														model.set("filestatus",SWFUpload.FILE_STATUS.CANCELLED);
														model.commit();
														var stats=this.swfupload.getStats();
														var label=Ext.getCmp("queue_id");
														label.setText(label.text="队列中文件个数:"+stats.files_queued);
													},
										icon: "../../../../Scripts/Ext4.0/swfuploadForExtJs/image/icons/remove.png"

												}],
										listeners:{
											show:function(myself,o){
												if(model.get("filestatus")!=-1){
													myself.items.getAt(0).setDisabled(true);
												}
												if(model.get("filestatus")==-4){
													myself.items.getAt(1).setDisabled(true);
												}
											}
										}
									});
							e.preventDefault(); 
    				  rightClick.showAt(e.getXY()); 
					console.log("鼠标右键点击");
				}
			},
			xtype : "grid",
			border : false,
			store : Ext.create("Ext.data.Store",{
				model:"Org.fileupload.FileModel",
				storeId:"fileItems"
			}),
			columns : [ 
			    new Ext.grid.RowNumberer(), {
				header : '文件名',
				width : 100,
				sortable : true,
				dataIndex : 'name',
				menuDisabled : true
			}, {
				header : '类型',
				width : 70,
				sortable : true,
				dataIndex : 'type',
				menuDisabled : true
			}, {
				header : '大小',
				width : 100,
				sortable : true,
				dataIndex : 'size',
				menuDisabled : true,
				renderer : this.formatFileSize
			}, {
				header : '进度',
				width : 150,
				sortable : true,
				dataIndex : 'percent',
				menuDisabled : true,
				renderer:this.formatProgress,
				scope:this,
				hidden:true
			}, {
				header : '状态',
				width : 100,
				sortable : true,
				dataIndex : 'filestatus',
				renderer:this.formatFileState,
				scope : this
			}, {
				header : '&nbsp;',
				width : 40,
				dataIndex : 'id',
				menuDisabled : true,
				renderer : this.formatDelBtn,
				hidden:true
			} ]
		
		}];
		this.tbar=[
			{ text: '添加文件', id: "btnAdd", icon: "../../../../Scripts/Ext4.0/swfuploadForExtJs/image/icons/add.png" }, '-',
			{text:'上传全部未开始上传文件',handler:function(){
				var store = Ext.data.StoreManager.lookup("fileItems");
				this.continuous=true;
				for (var index = 0; index <store.getCount(); index++) {
					var model=	store.getAt(index);
					if(model.get("filestatus")==-1){
						this.swfupload.startUpload(model.get("id"));
						model.set("filestatus",-2);
						model.commit();
					}
				}
}, scope: this, icon: "../../../../Scripts/Ext4.0/swfuploadForExtJs/image/icons/upload.gif"
}, '-',
			{
					text : '停止上传',
					handler : function() {
						this.swfupload.stopUpload();

					},
		icon: "../../../../Scripts/Ext4.0/swfuploadForExtJs/image/icons/cancel.png",
					scope : this
			}, '-', {
					text : '请空列表',
					handler : function() {
						this.swfupload.cancelQueue();
						var store = Ext.data.StoreManager.lookup("fileItems");
						store.removeAll();
						var stats=this.swfupload.getStats();
						var label=Ext.getCmp("queue_id");
						label.setText(label.text="队列中文件个数:"+stats.files_queued);
					},
		icon: "../../../../Scripts/Ext4.0/swfuploadForExtJs/image/icons/remove.png",
					scope:this
			}, '-', {
					xtype : 'label',
					id:"queue_id",
					text : '队列中文件个数:0',
					margins : '0 0 0 10'
			}
		 ];
		this.bbar=[
		{xtype:"progressbar",id:"progressBar",text:"0%",width:200},
		{xtype:"label",text:"平均速度：0kb/s",id:"currentSpeed",width:200},'-',
		{xtype:"label",text:"剩余时间：0s",id:"timeRemaining",width:200}
		];
		this.listeners={
			'afterrender':function(){
			//	console.log("渲染完成， 添加swf所需的设置");
				var em=Ext.get(Ext.query("#btnAdd>em")[0]);
				if(!em){
					//此处为IE9一下版本的兼容问题的临时解决办法，目前还是不支持IE6
					em=Ext.get("btnAdd-btnWrap");
				}
				var placeHolderId = Ext.id();
				em.setStyle({
					position : 'relative',
					display : 'block'
				});
				em.createChild({
					tag : 'div',
					id : placeHolderId
				});
				this.swfupload = new SWFUpload(Ext.apply(this.setting,{
					button_width : em.getWidth(),
					button_height : em.getHeight(),
					button_placeholder_id :placeHolderId
				}));
				this.swfupload.uploadStopped = false;
				Ext.get(this.swfupload.movieName).setStyle({
					position : 'absolute',
					left :"0px"
				});	
			}
		};
		
		this.callParent();
		scope : this;
		delay : 100;
	},
	onQueued:function(file){
		var stats=this.getStats();
		var label=Ext.getCmp("queue_id");
		label.setText(label.text="队列中文件个数:"+stats.files_queued);
		var f=Ext.create("Org.fileupload.FileModel",{
			id: file.id,
			name : file.name,
			type : file.type,
			size : file.size,
			filestatus : file.filestatus,
			percent : 0
		});
		Ext.data.StoreManager.lookup("fileItems").add(f);
	},
	formatFileState : function(n){//文件状态
		switch(n){
			case SWFUpload.FILE_STATUS.QUEUED  : return '已加入队列';
			break;
			case SWFUpload.FILE_STATUS.IN_PROGRESS  : return '正在上传';
			break;
			case SWFUpload.FILE_STATUS.ERROR  : return '<div style="color:red;">上传失败</div>';
			break;
			case SWFUpload.FILE_STATUS.COMPLETE  : return '上传成功';
			break;
			case SWFUpload.FILE_STATUS.CANCELLED  : return '取消上传';
			break;
			default: return n;
		}
	},
	onFileError : function(file,errorCode,msg){
		var msg="";
		console.log(errorCode);
		switch(errorCode){
			case SWFUpload.QUEUE_ERROR.QUEUE_LIMIT_EXCEEDED : msg='待上传文件列表数量超限，不能选择！';
			break;
			case SWFUpload.QUEUE_ERROR.FILE_EXCEEDS_SIZE_LIMIT : msg='文件太大，不能选择！文件大小不能超过'+this.settings.file_size_limit/1024+'MB';
			break;
			case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE : msg='该文件大小为0，不能选择！';
			break;
			case SWFUpload.QUEUE_ERROR.INVALID_FILETYPE : msg='该文件类型不可以上传！';
			break;
			case SWFUpload.UPLOAD_ERROR.UPLOAD_STOPPED : msg="上传已经停止";
			break;
			case SWFUpload.UPLOAD_ERROR.FILE_CANCELLED:msg="所有文件已经取消！";
			break;
			default:msg="未知错误!";
			break;
		}
		Ext.Msg.show({
				title : '提示',
				msg : msg,
				width : 280,
				icon : Ext.Msg.WARNING,
				buttons :Ext.Msg.OK
		
		});
	},
	uploadProgress : function(file, bytesComplete, totalBytes){//处理进度条
		console.log("完成百分比"+file.percentUploaded+"，当前速度"+file.currentSpeed/8/1024/1024+"MB/s");
		console.log(SWFUpload.speed.formatBytes(bytesComplete));
       	var ds = Ext.data.StoreManager.lookup("fileItems");
		for(var i=0;i<ds.getCount();i++){
			var record =ds.getAt(i);
			if(record.get('id')==file.id){
				record.set('percent', file.percentUploaded);
				record.set('filestatus', file.filestatus);
				record.commit();
			}
		}
		//更新进度条
		var pb=Ext.getCmp("progressBar");
		pb.updateProgress(file.percentUploaded/100,SWFUpload.speed.formatPercent(file.percentUploaded),true);
		//更新当前速度
		var speed=Ext.getCmp("currentSpeed");
		var speedNum=Math.ceil(file.averageSpeed/8/1024);
		console.log(Math.ceil(2.8));
		var unit=speedNum/1024<0?"KB/s":"MB/s";
		var speedValue=speedNum/1024<0?speedNum:speedNum/1024;
		speedValue=Math.ceil(speedValue);
		speed.setText("平均速度:"+speedValue+unit);
		//更新剩余时间
		var timeRemaining=Ext.getCmp("timeRemaining");
		timeRemaining.setText("估计剩余时间:"+SWFUpload.speed.formatTime(file.timeRemaining));
		
	},
	uploadComplete:function(file){
		var store = Ext.data.StoreManager.lookup("fileItems");
		model=store.getById(file.id);
		model.set("filestatus",file.filestatus);
		model.commit();
		console.log(this.customSettings.scope_handler.continuous);
		var stats=this.getStats();
		var label=Ext.getCmp("queue_id");
		label.setText(label.text="队列中文件个数:"+stats.files_queued);
		return this.customSettings.scope_handler.continuous;
	}
});

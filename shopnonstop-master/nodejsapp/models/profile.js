
var mongoose = require('mongoose');
var uniqueValidator = require('mongoose-unique-validator');
var Schema = mongoose.Schema;


var ProfileSchema = new Schema({name:{type: String, default: ''}, 
	email:{type: String, unique: true, required: true},mobile:{type: Number},gender:{type: String},street:{type: String},
	landmark:{type: String},city:{type: String},state:{type: String},country:{type:String},pincode:{type: String},userid:{type: Schema.Types.ObjectId}});
ProfileSchema.plugin(uniqueValidator);



module.exports = mongoose.model('Profile',ProfileSchema);
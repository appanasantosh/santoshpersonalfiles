class CreateUsers < ActiveRecord::Migration
  def change
    create_table :users do |t|
      t.string :name
      t.string :email
      t.string :password_digest
      t.date :dateofbirth
      t.string :gender
      t.string :qualification
      t.string :occupation
      t.string :country
      t.attachment :userimage

      t.timestamps null: false
    end
  end
end

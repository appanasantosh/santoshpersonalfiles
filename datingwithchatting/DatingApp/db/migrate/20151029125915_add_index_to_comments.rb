class AddIndexToComments < ActiveRecord::Migration
  def change
  	add_index :comments, [:micropost_id, :created_at]
  end
end

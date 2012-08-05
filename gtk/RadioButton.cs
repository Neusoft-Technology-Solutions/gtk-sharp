//
//  RadioButton.cs
//
//	Author:  John Luke  <jluke@cfl.rr.com>
//
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of version 2 of the Lesser GNU General 
// Public License as published by the Free Software Foundation.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this program; if not, write to the
// Free Software Foundation, Inc., 59 Temple Place - Suite 330,
// Boston, MA 02111-1307, USA.

namespace Gtk {

	using System;
	using System.Runtime.InteropServices;

	public partial class RadioButton {

		[DllImport ("libgtk-win32-3.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr gtk_radio_button_new_with_mnemonic (IntPtr group, IntPtr label);

		// creates a new group for this RadioButton
		public RadioButton (string label)
		{
			IntPtr native = GLib.Marshaller.StringToPtrGStrdup (label);
			Raw = gtk_radio_button_new_with_mnemonic (IntPtr.Zero, native);
			GLib.Marshaller.Free (native);
		}
	}
}

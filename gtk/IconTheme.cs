// IconTheme.cs - customizations to Gtk.IconTheme
//
// Authors: Mike Kestner  <mkestner@ximian.com>
//	    Jeroen Zwartepoorte  <jeroen@xs4all.nl>
//
// Copyright (c) 2004-2005 Novell, Inc.
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
	using System.Collections;
	using System.Runtime.InteropServices;

	public partial class IconTheme {

		[DllImport ("libgtk-win32-3.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr gtk_icon_theme_list_icons (IntPtr raw, IntPtr context);

		public string[] ListIcons (string context)
		{
			IntPtr native = GLib.Marshaller.StringToPtrGStrdup (context);
			IntPtr list_ptr = gtk_icon_theme_list_icons (Handle, native);
			GLib.Marshaller.Free (native);
			if (list_ptr == IntPtr.Zero)
				return new string [0];

			GLib.List list = new GLib.List (list_ptr, typeof (string), true, true);
			string[] result = new string [list.Count];
			int i = 0;
			foreach (string val in list)
				result [i++] = val;
			return result;
		}

		[DllImport ("libgtk-win32-3.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern void gtk_icon_theme_get_search_path(IntPtr raw, out IntPtr path, out int n_elements);

		[DllImport ("libgtk-win32-3.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern void gtk_icon_theme_set_search_path(IntPtr raw, IntPtr[] path, int n_elements);

		[DllImport ("libgtk-win32-3.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern void gtk_icon_theme_get_search_path_utf8(IntPtr raw, out IntPtr path, out int n_elements);

		[DllImport ("libgtk-win32-3.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern void gtk_icon_theme_set_search_path_utf8(IntPtr raw, IntPtr[] path, int n_elements);

		[DllImport ("libglib-2.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern void g_strfreev (IntPtr mem);

		bool IsWindowsPlatform {
			get {
				switch (Environment.OSVersion.Platform) {
				case PlatformID.Win32NT:
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.WinCE:
					return true;
				default:
					return false;
				}
			}
		}

		public string[] SearchPath {
			get {
				string[] retval;

				unsafe {
					int length;
					IntPtr raw_ret;
					if (IsWindowsPlatform)
						gtk_icon_theme_get_search_path_utf8 (Handle, out raw_ret, out length);
					else
						gtk_icon_theme_get_search_path (Handle, out raw_ret, out length);

					int size = Marshal.SizeOf (typeof (IntPtr));
					retval = new string[length];
					for (int i = 0, j = 0; i < length; i++, j += size) {
						IntPtr string_ptr = Marshal.ReadIntPtr (new IntPtr (raw_ret.ToInt32 () + j));
						retval[i] = GLib.Marshaller.Utf8PtrToString (string_ptr);
					}

					g_strfreev (raw_ret);
				}

				return retval;
			}
			set {
				int cnt_path = value == null ? 0 : value.Length;
				IntPtr[] native_path = new IntPtr [cnt_path];
				for (int i = 0; i < cnt_path; i++)
					native_path [i] = GLib.Marshaller.StringToPtrGStrdup (value[i]);

				if (IsWindowsPlatform)
					gtk_icon_theme_set_search_path_utf8 (Handle, native_path, native_path.Length);
				else
					gtk_icon_theme_set_search_path (Handle, native_path, native_path.Length);

				for (int i = 0; i < native_path.Length; i++)
					GLib.Marshaller.Free (native_path[i]);
			}
		}

		[Obsolete ("Replaced by SearchPath property.")]
		public void SetSearchPath (string[] path)
		{
			SearchPath = path;
		}

		[DllImport ("libgtk-win32-3.0-0.dll", CallingConvention = CallingConvention.Cdecl)]
		static extern IntPtr gtk_icon_theme_get_icon_sizes (IntPtr raw, IntPtr icon_name);

		public int[] GetIconSizes (string icon_name) 
		{
			IntPtr icon_name_as_native = GLib.Marshaller.StringToPtrGStrdup (icon_name);
			IntPtr raw_ret = gtk_icon_theme_get_icon_sizes(Handle, icon_name_as_native);
			ArrayList result = new ArrayList ();
			int offset = 0;
			int size = Marshal.ReadInt32 (raw_ret, offset);
			while (size != 0) {
				result.Add (size);
				offset += 4;
				size = Marshal.ReadInt32 (raw_ret, offset);
			}
			GLib.Marshaller.Free (icon_name_as_native);
			GLib.Marshaller.Free (raw_ret);
			return (int[]) result.ToArray (typeof (int));
		}
	}
}
